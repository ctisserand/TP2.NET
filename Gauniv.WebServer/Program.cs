using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using Gauniv.WebServer.Security;
using Gauniv.WebServer.Services;
using Gauniv.WebServer.Websocket;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


// Set the culture so that the culture is the same between front and back
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(s =>
{
    s.SupportedCultures =
    [
        cultureInfo
    ];
    s.SupportedUICultures =
    [
        cultureInfo
    ];
});


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // chemin pour la page de connexion
        options.LogoutPath = "/Account/Logout";  // chemin pour la déconnexion
    });


builder.Services.AddDefaultIdentity<User>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSignalR();
builder.Services.AddHostedService<OnlineService>();
builder.Services.AddHostedService<SetupService>();
// var redis = new  RedisService(redisConnectionString);
// builder.Services.AddSingleton(redis);
/*
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents()
    {
        OnRedirectToLogin = (ctx) =>
        {
            if (ctx.Request.Path.StartsWithSegments("api") && ctx.Response.StatusCode == 200)
            {
                ctx.Response.StatusCode = 401;
            }
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = (ctx) =>
        {
            if (ctx.Request.Path.StartsWithSegments("api") && ctx.Response.StatusCode == 200)
            {
                ctx.Response.StatusCode = 403;
            }
            return Task.CompletedTask;
        }
    };
});*/

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
app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=AdminPanel}/{action=AdminIndex}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "SeeCategory",
    pattern: "{controller=Category}/{action=See}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "Lybrary",
    pattern: "{controller=Lybrary}/{action=LybraryIndex}/{id?}")
    .WithStaticAssets();





app.MapRazorPages()
   .WithStaticAssets();

app.MapOpenApi();
app.MapGroup("Bearer").MapPost("/login", async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>>
            ([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<User>>();

            var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(login.TwoFactorCode))
                {
                    result = await signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
                }
                else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                {
                    result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                }
            }

            if (!result.Succeeded)
            {
                return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
            }

            // The signInManager already produced the needed response in the form of a cookie or bearer token.
            return TypedResults.Empty;
        })
.WithOpenApi();


app.UseAuthentication();  // Assurez-vous que l'authentification est bien activée
app.UseAuthorization();



app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});
app.MapHub<OnlineHub>("/online");
app.Run();
