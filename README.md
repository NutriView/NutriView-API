# NutriView.API

ASP.NET Core Web API for NutriView, a calorie-tracking application.

## Authentication

The API uses **JWT bearer tokens**. `POST /api/User/register` and `POST /api/User/login`
return `{ token, expiresAt, user }`; every other endpoint requires
`Authorization: Bearer <token>`.

Endpoints never take a user id — the caller is resolved from the token's `sub` claim,
and user-owned rows (food entries, reminders, uploaded images, nutrition goal) are
queried scoped to that id. A request for someone else's row returns `404`, so the API
does not reveal that it exists.

Passwords are hashed with `PasswordHasher<User>` (PBKDF2-HMAC-SHA512, per-user random
salt). Plaintext is never stored and `PasswordHash` is never returned.

### Configuration

`Jwt:Issuer`, `Jwt:Audience` and `Jwt:ExpiryDays` live in `appsettings.json`.
`Jwt:Key` is the symmetric signing key and must be **at least 32 bytes** — the app
refuses to start otherwise.

`appsettings.Development.json` carries a development-only key so `dotnet run` works out
of the box. **Do not use it outside development.** Supply a real key in production via
user secrets or the environment:

```
dotnet user-secrets set "Jwt:Key" "<a long random value>"
# or
setx Jwt__Key "<a long random value>"
```

## Running

```
dotnet run --launch-profile https     # https://localhost:5000, http://localhost:5010
```

Swagger UI is served at `/swagger` in development; use its **Authorize** button to
paste a token from `/api/User/login`.
