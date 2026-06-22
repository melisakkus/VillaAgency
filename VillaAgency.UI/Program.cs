using FluentValidation.AspNetCore;
using VillaAgency.Business.Extension;
using VillaAgency.Business.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES
builder.Services.AddControllersWithViews();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMemoryCache();

builder.Services.AddBusinessServices(builder.Configuration);

var app = builder.Build();

// 2. MIDDLEWARE PIPELINE
// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");

app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 3. ROUTING & ENDPOINTS
app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
