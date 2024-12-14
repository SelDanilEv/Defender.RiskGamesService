using Defender.Common.Extension;
using Defender.RiskGamesService.Application;
using Defender.RiskGamesService.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

builder.Logging.ClearProviders()
    .AddSerilog(logger)
    .AddConsole();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration)
    .AddWebUIServices(builder.Environment, builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration, builder.Environment);

var app = builder.Build();

if (builder.Environment.IsLocalOrDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    app.UseDefaultFiles();
    app.UseStaticFiles();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseProblemDetails();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}");

await app.RunAsync();
