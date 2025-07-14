using LeaveManagement.API.Extensions;
using LeaveManagement.Identity;
using LeaveManagement.Application;
using LeaveManagement.Persistence;

using Serilog;
using System.Text.Json;
using LeaveManagement.API.Extentions;
using LeaveManagement.API.Data;
using LeaveManagement.Identity.Models;
using LeaveManagement.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.API.Attribute;
using LeaveManagement.API.Services;
using LeaveManagement.Application.Interfaces.Services;
using LeaveManagement.Identity.Contexts;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    ContentRootPath = Directory.GetCurrentDirectory(),
    Args = args
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

ConfigurationManager _config = builder.Configuration;

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfrastructure(_config);
builder.Services.AddIdentityInfrastructure(_config);
builder.Services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerExtension();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddApiVersioningExtension();
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddResponseCaching();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Log/LeaveManagement.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerExtension();
app.UseErrorHandlingMiddleware();
app.UseHealthChecks("/health");
app.UseResponseCaching();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var applicationContext = services.GetRequiredService<ApplicationDbContext>();
        var identityContext = services.GetRequiredService<IdentityContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await applicationContext.Database.MigrateAsync();
        await identityContext.Database.MigrateAsync();

        await SeedData.SeedAsync(applicationContext, identityContext, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration or seeding");
    }
}

app.Run();