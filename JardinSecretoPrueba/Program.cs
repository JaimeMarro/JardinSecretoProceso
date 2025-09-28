using JardinSecretoPrueba1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
//Conexion a la base de datos
builder.Services.AddDbContext<JardinSecretoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));

//Para Cookies
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
     {
         options.LoginPath = "/Auth/Login"; //Este funciona por si alguien intenta entrar a la url y no esta logueado lo reenvia al login
         options.AccessDeniedPath = "/Auth/Denied"; //Este es si no tiene permisos
         options.ExpireTimeSpan = TimeSpan.FromHours(1); //El tiempo que podra tener dentro del sistema como admin
         options.SlidingExpiration = true;
         options.Cookie.HttpOnly = true;
         options.Cookie.SameSite = SameSiteMode.Lax;

     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
