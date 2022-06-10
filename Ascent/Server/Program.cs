using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;
var configuration = builder.Configuration;
var services = builder.Services;

// In production, this app will sit behind a Nginx reverse proxy with HTTPS
if (!environment.IsDevelopment())
    builder.WebHost.UseUrls("http://localhost:5002");

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// Configure Services

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = configuration["OIDC:Authority"];
    options.Audience = configuration["OIDC:Audience"];
    // options.RequireHttpsMetadata = false;
});

services.AddControllersWithViews();
services.AddRazorPages();
services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

// Build App

var app = builder.Build();

// Configure Middleware Pipeline

app.UseForwardedHeaders();

if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

// Run App

app.Run();
