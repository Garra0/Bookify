using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // the next lines help me to have multiple versions of the same controller in the swagger UI
        // and also to have the versioning in the URL of the swagger.json file
        // and i cant select the version of the API i want to test in the swagger UI
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });

    app.ApplyMigrations();

    app.SeedData();
}

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.MapControllers();

// Health check endpoint -> https://localhost:5000/health ->
// its tell me if the DB is up "Healthy" or not "Unhealthy" and
// also the response is in JSON format with the status of the health check. 
// في اكثر من بكج للهيلث شيك مشان احدد الشيك لمين اللي اخر امتداده بيكون 
// Uris فهو للكي كلوك
// في واحد كمان نزلته للريديس ونزلت واحد للداتا بيس Npgsql وطبعا كلهم بالانفراستركشر
// "AddHealthChecks()"
app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
