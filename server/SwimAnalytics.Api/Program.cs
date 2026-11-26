using Microsoft.EntityFrameworkCore;
using SwimAnalytics.Application.Services;
using SwimAnalytics.Data;
using SwimAnalytics.Data.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISwimmerService, SwimmerService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IMeetService, MeetService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IRelayService, RelayService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy => policy
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Angular");
app.MapControllers();

app.Run();