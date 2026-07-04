using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Serilog;
using VillaAgency.Business.Extension;
using VillaAgency.Entity.Identity;
using VillaAgency.Entity.Identity.Constants;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

// Instruct .NET to use Serilog as the logging provider
builder.Host.UseSerilog();

// Prevents null values in DTOs from overwriting existing valid data in target entities during mapping
TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

// Services registration
builder.Services.AddControllersWithViews(options =>
{   //Disables .NET's automatic [Required] validation for non-nullable reference types (like string) via MvcOptions
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddBusinessServices(builder.Configuration);

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

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
app.UseAuthentication();
app.UseAuthorization();

// Routing & Endpoints 
app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    foreach (var roleName in new[] { Roles.Admin, Roles.Manager })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new AppRole(roleName));
    }

    var adminEmail = "admin@villaagency.com";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new AppUser
        {
            UserName = "admin",
            Email = adminEmail,
            FullName = "VillaAgency Admin",
            IsActive = true
        };
        var result = await userManager.CreateAsync(admin, "admin00");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}

app.Run();
