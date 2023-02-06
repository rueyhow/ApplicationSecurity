using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyConnection");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDataProtection();
builder.Services.AddDbContext<FreshFarmMarketContext>();
builder.Services.AddSingleton<IHttpContextAccessor , HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".UserInfo.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(5);
    options.Cookie.IsEssential = true;
});
builder.Services.AddTransient<reCaptchaService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddTransient<PasswordService>();
builder.Services.AddTransient<auditService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 3;
});
builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
);
builder.Services.AddIdentity<MarketUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<FreshFarmMarketContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
});
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options
=>
{
    options.Cookie.Name = "MyCookieAuth";
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "739847602809-2mmsi9vbgih5hri32tpe600m61p4tf01.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});
builder.Services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithRedirects("/Error?id={0}");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();


app.Run();
