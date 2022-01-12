using AdminHubNC6.Areas.Identity.Data;
using AdminHubNC6.Data;
using AdminHubNC6.Resources;
using InfoHub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Default SQL Connection
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//AuthDb SQL Connection
var AuthDbContextConnection = builder.Configuration.GetConnectionString("AuthDbContextConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(AuthDbContextConnection));

builder.Services.AddIdentity<AdminHubUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

//TodoEvent SQL Connection
var TodoEventConnectionString = builder.Configuration.GetConnectionString("TodoEventDbContextConnection");
builder.Services.AddDbContext<TodoEventDbContext>(options =>
    options.UseSqlServer(TodoEventConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddSingleton<AuthLocalizationService>();
builder.Services.AddSingleton<LocalizationService>();
builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

builder.Services.Configure<RequestLocalizationOptions>(
              opts =>
              {
                  var supportedCultures = new List<CultureInfo>
                  {
                        new CultureInfo("en"),
                        new CultureInfo("zh-Hant"),
                        new CultureInfo("zh-Hans")
                  };

                  opts.DefaultRequestCulture = new RequestCulture("en");
                  // Formatting numbers, dates, etc.
                  opts.SupportedCultures = supportedCultures;
                  // UI strings that we have localized.
                  opts.SupportedUICultures = supportedCultures;

                  opts.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
              });

builder.Services.AddRazorPages().AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(AuthResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("AuthResource", assemblyName.Name);
                    };
                });


//Role policy setup
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("readpolicy",
        builder => builder.RequireRole("Admin", "Level1", "Level2", "Level3", "Guest"));
    options.AddPolicy("writepolicy",
        builder => builder.RequireRole("Admin", "Level3"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var supportedCultures = new[] { "en", "zh-Hant", "zh-Hans" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();