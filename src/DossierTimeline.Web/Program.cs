#nullable enable

using System.Security.Claims;
using DossierTimeline.Web.Data;
using DossierTimeline.Web.Models;
using DossierTimeline.Web.Services;
using DossierTimeline.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//------------------------------------
// Configuration and Logging
//------------------------------------

// Ensure appsettings.json has "ConnestionStrings:Default"
// Ensure there is an admin bootstrapped user in appsettings.json under "Admin:email" and "Admin:Password"

var configuration = builder.Configuration;
var services = builder.Services;

//-------------------------------------
// Databasing
//-------------------------------------

services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlite(configuration.GetConnectionString("Deafult") ?? "Data Source=dossier.db");
});

//-------------------------------------
// Identity and User Roles
//-------------------------------------

services
    .AddDefaultIdentity<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLength = 8;
        options.Password.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

services.AddAuthentication(options =>
{
    options.DefaultAuthenticationScheme = IdentityConstraints.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstraints.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstraints.ApplicationScheme;
}).AddCookie(IdentityContraints.ApplicationScheme);

services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

//--------------------------------------
// Plumbing
//--------------------------------------

services.AddRazorPages();
services.AddServerSideBlazor();

// Handle large upload sizes for various files and archives

services.Configure<FromOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

//--------------------------------------
// Domain Items
//--------------------------------------

services.AddScoped<IDocumentServices, DocumentService>();
services.AddScoped<IAuditService, AuditService>();
services.AddScoped<IFileStorage, LocalFileStorage>();
services.AddScoped<ISearchIndex, LuceneSearchIndex>();
services.AddScoped<ISettingsService, SettingsService>();

// ViewModels
services.AddScoped<LoginViewModel>();
services.AddScoped<AdminDashboardViewModel>();
services.AddScoped<UserDashboardViewModel>();
services.AddScoped<UploadDocsViewModel>();
services.AddScoped<OrganizeDocsViewModel>();
services.AddScoped<ViewSearchDocsViewModel>();
services.AddScoped<ManageDocsViewModel>();
services.AddScoped<ScanDocsViewModel>();
services.AddScoped<GeneralSettingsViewModel>();
services.AddScoped<AdminSettingsViewModel>();
services.AddScoped<AuditTrailViewModel>();


var app = builder.Build();


// -------------------------------
// Startup Pipeline
// -------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

// Seed DB, apply migrations, bootstrap roles/admin user
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();


    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleMgr.RoleExistsAsync("Admin")) await roleMgr.CreateAsync(new IdentityRole("Admin"));
    if (!await roleMgr.RoleExistsAsync("User")) await roleMgr.CreateAsync(new IdentityRole("User"));


    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var adminEmail = configuration["Admin:Email"];
    var adminPassword = configuration["Admin:Password"];


    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser { UserName = adminEmail, Email = adminEmail, DisplayName = "Administrator" };
            var created = await userMgr.CreateAsync(admin, adminPassword);
            if (created.Succeeded)
            {
                await userMgr.AddToRoleAsync(admin, "Admin");
            }
        }
        else
        {
            // ensure role
            if (!await userMgr.IsInRoleAsync(admin, "Admin"))
                await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
}


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.Run();