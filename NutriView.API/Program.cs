using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NutriView.API.Data;
using NutriView.API.Services;

var builder = WebApplication.CreateBuilder(args);

const string SpaCorsPolicy = "spa";

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serialize enums (Gender, MeasurementBase) as their names instead of
        // integers so the API contract and generated clients are self-documenting.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    // Allow the Vite dev server (and any SPA origin) to call the API from the browser.
    // No credentials are used (the client holds the user in localStorage, not cookies).
    options.AddPolicy(SpaCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFoodEntryService, FoodEntryService>();
builder.Services.AddScoped<IUploadedImageService, UploadedImageService>();
builder.Services.AddScoped<IReminderService, ReminderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(SpaCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();