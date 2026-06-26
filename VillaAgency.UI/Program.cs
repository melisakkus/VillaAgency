using FluentValidation.AspNetCore;
using Serilog;
using VillaAgency.Business.Extension;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

// Instruct .NET to use Serilog as the logging provider
builder.Host.UseSerilog();

// Services registration
builder.Services.AddControllersWithViews(options =>
{   //Disables .NET's automatic [Required] validation for non-nullable reference types (like string) via MvcOptions
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddBusinessServices(builder.Configuration);

var app = builder.Build();

// Ensure logs are safely flushed and closed when the application shuts down
app.Lifetime.ApplicationStopping.Register(Log.CloseAndFlush);

// Middleware pipeline 
app.UseExceptionHandler("/Home/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Enables automatic logging of all HTTP requests (page requests, API calls, etc.)
app.UseSerilogRequestLogging();

app.UseRouting();
app.UseAuthorization();

// Routing & Endpoints 
app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
