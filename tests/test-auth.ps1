# End-to-end smoke test for NutriView's JWT authentication and per-user isolation.
#
# Start the API first, then run this from the repository root:
#
#     cd NutriView.API
#     dotnet run --launch-profile https
#
#     .\tests\test-auth.ps1
#
# It expects the API on http://localhost:5010 (the "https" launch profile serves
# plain http there as well). Two throwaway accounts are created and deleted again
# at the end, so it is safe against a development database.
#
# The stored-hash checks shell out to sqlcmd; without it on PATH they are skipped
# and the remaining checks still run.

$api = 'http://localhost:5010/api'

# Both accounts share a password on purpose: the DB check at the end proves the
# stored hashes still differ, which is what per-user salting buys you.
$password = 'SharedPassw0rd!'
$emailA = "test-a-$(Get-Random)@nutriview.test"
$emailB = "test-b-$(Get-Random)@nutriview.test"

$script:pass = 0
$script:fail = 0

function Req {
    param($Method, $Path, $Body, $Token)

    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }

    $params = @{
        Method          = $Method
        Uri             = "$api$Path"
        Headers         = $headers
        ContentType     = 'application/json'
        UseBasicParsing = $true
    }
    if ($null -ne $Body) { $params['Body'] = ($Body | ConvertTo-Json -Compress) }

    try {
        $r = Invoke-WebRequest @params
        $parsed = $null
        if ($r.Content) { try { $parsed = $r.Content | ConvertFrom-Json } catch {} }
        return @{ Code = [int]$r.StatusCode; Data = $parsed }
    }
    catch {
        $code = 0
        if ($_.Exception.Response) { $code = [int]$_.Exception.Response.StatusCode }
        return @{ Code = $code; Data = $null }
    }
}

function Check($label, $expected, $actual) {
    if ($expected -eq $actual) {
        Write-Host ("  PASS  {0,-45} {1}" -f $label, $actual) -ForegroundColor Green
        $script:pass++
    }
    else {
        Write-Host ("  FAIL  {0,-45} got {1}, expected {2}" -f $label, $actual, $expected) -ForegroundColor Red
        $script:fail++
    }
}

Write-Host "`n== 1. Endpoints are closed by default ==" -ForegroundColor Cyan
Check 'GET /FoodEntry/me with no token' 401 (Req GET '/FoodEntry/me').Code
Check 'GET /User/me with no token'      401 (Req GET '/User/me').Code
Check 'GET /Food with no token'         401 (Req GET '/Food').Code

Write-Host "`n== 2. Register and log in ==" -ForegroundColor Cyan
$ra = Req POST '/User/register' @{ email = $emailA; password = $password; nickName = 'Test A'; dailyCalorieGoal = 2200 }
Check 'register user A' 201 $ra.Code
$tokenA = $ra.Data.token

$rb = Req POST '/User/register' @{ email = $emailB; password = $password; nickName = 'Test B'; dailyCalorieGoal = 1800 }
Check 'register user B' 201 $rb.Code
$tokenB = $rb.Data.token

Check 'token has 3 segments' 3 ($tokenA -split '\.').Count
Check 'the two users got different tokens' $true ($tokenA -ne $tokenB)
Check 'response carries no password hash' $true ($ra.Data.user.PSObject.Properties.Name -notcontains 'passwordHash')

Check 'duplicate email is rejected' 400 (Req POST '/User/register' @{ email = $emailA; password = $password; nickName = 'Dupe'; dailyCalorieGoal = 2000 }).Code
Check 'login with correct password'  200 (Req POST '/User/login' @{ email = $emailA; password = $password }).Code
Check 'login with wrong password'    401 (Req POST '/User/login' @{ email = $emailA; password = 'wrong' }).Code
Check 'login with unknown email'     401 (Req POST '/User/login' @{ email = 'nobody@nowhere.test'; password = $password }).Code

Write-Host "`n== 3. A valid token identifies the caller ==" -ForegroundColor Cyan
$me = Req GET '/User/me' -Token $tokenA
Check 'GET /User/me with token' 200 $me.Code
Check '/me returns the right user' $emailA $me.Data.email
Check 'tampered token is rejected' 401 (Req GET '/User/me' -Token ($tokenA.Substring(0, $tokenA.Length - 4) + 'AAAA')).Code
Check 'garbage token is rejected'  401 (Req GET '/User/me' -Token 'not-a-token').Code

Write-Host "`n== 4. One user cannot touch another's data ==" -ForegroundColor Cyan
$food = Req POST '/Food' @{
    name      = "Test Food $(Get-Random)"
    isGlobal  = $false
    nutrition = @{ protein = 10; carbs = 20; fat = 5; sugar = 2; fiber = 1; sodium = 50; alcohol = 0; measurementBase = 'Per100g' }
} -Token $tokenA
Check 'A creates a food' 201 $food.Code
$foodId = $food.Data.foodId

$entry = Req POST '/FoodEntry' @{ foodId = $foodId; mealId = 1; quantity = 150; unit = 'g'; entryDate = '2026-08-17T08:00:00Z' } -Token $tokenA
Check 'A creates a food entry' 201 $entry.Code
$entryId = $entry.Data.foodEntryId

Check 'A reads own entry'        200 (Req GET "/FoodEntry/$entryId" -Token $tokenA).Code
Check "B reads A's entry"        404 (Req GET "/FoodEntry/$entryId" -Token $tokenB).Code
Check "B updates A's entry"      404 (Req PUT "/FoodEntry/$entryId" @{ mealId = 2; quantity = 999; entryDate = '2026-08-17T08:00:00Z' } -Token $tokenB).Code
Check "B deletes A's entry"      404 (Req DELETE "/FoodEntry/$entryId" -Token $tokenB).Code
Check "A's entry survived"       200 (Req GET "/FoodEntry/$entryId" -Token $tokenA).Code
Check "B's own list is empty"    0   (Req GET '/FoodEntry/me' -Token $tokenB).Data.Count
Check "A's own list has 1 entry" 1   (Req GET '/FoodEntry/me' -Token $tokenA).Data.Count

Write-Host "`n== 5. Goals and reminders are per-user too ==" -ForegroundColor Cyan
Check 'A has no goal yet' 404 (Req GET '/User/me/nutrition-goal' -Token $tokenA).Code
Check 'A sets a goal'     204 (Req PUT '/User/me/nutrition-goal' @{ protein = 140; carbs = 220; fat = 70; sugar = 40; fiber = 30; sodium = 2300; alcohol = 0; measurementBase = 'Per100g' } -Token $tokenA).Code

$goal = Req GET '/User/me/nutrition-goal' -Token $tokenA
Check 'A reads the goal back' 200 $goal.Code
Check 'calories were computed server-side' 2010 $goal.Data.calories
Check "B still has no goal"   404 (Req GET '/User/me/nutrition-goal' -Token $tokenB).Code

$rem = Req POST '/Reminder' @{ mealId = 1; timeOfDay = '08:30:00'; isActive = $true } -Token $tokenA
Check 'A creates a reminder' 201 $rem.Code
Check "B reads A's reminder" 404 (Req GET "/Reminder/$($rem.Data.reminderId)" -Token $tokenB).Code
Check "B's reminder list is empty" 0 (Req GET '/Reminder/me' -Token $tokenB).Data.Count

Write-Host "`n== 6. Stored hashes (same password, different hash) ==" -ForegroundColor Cyan
$q = "SET NOCOUNT ON; SELECT PasswordHash FROM Users WHERE Email IN ('$emailA','$emailB')"
$hashes = @(sqlcmd -S "(localdb)\MSSQLLocalDB" -d NutriViewDb -I -h -1 -W -Q $q 2>$null | Where-Object { $_.Trim() -ne '' })
if ($hashes.Count -eq 2) {
    Check 'both hashes stored'                $true ($hashes[0].Length -gt 50 -and $hashes[1].Length -gt 50)
    Check 'identical passwords -> different hashes' $true ($hashes[0] -ne $hashes[1])
    Check 'PBKDF2 format marker'              $true ($hashes[0].StartsWith('AQAAAA'))
    Check 'plaintext password is not stored'  $true ($hashes[0] -notmatch [regex]::Escape($password))
}
else {
    Write-Host '  SKIP  could not read hashes (is sqlcmd on PATH?)' -ForegroundColor Yellow
}

Write-Host "`n== 7. Clean up ==" -ForegroundColor Cyan
Check 'delete the test food'   204 (Req DELETE "/Food/$foodId" -Token $tokenA).Code
Check 'A deletes own account'  204 (Req DELETE '/User/me' -Token $tokenA).Code
Check 'B deletes own account'  204 (Req DELETE '/User/me' -Token $tokenB).Code

Write-Host "`n---------------------------------------------" -ForegroundColor Cyan
if ($script:fail -eq 0) {
    Write-Host "ALL $($script:pass) CHECKS PASSED" -ForegroundColor Green
}
else {
    Write-Host "$($script:pass) passed, $($script:fail) FAILED" -ForegroundColor Red
}
Write-Host ''
