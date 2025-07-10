using street.Config;
using street.Data;
using street.Services;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using AspNetCore.Identity.Mongo;
using street.Models;
using AspNetCore.Identity.Mongo.Model;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração para o MongoDB e Identity
        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
        builder.Services.AddSingleton<MongoDbContext>();

        builder.Services.AddIdentity<ApplicationUser, MongoRole<string>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddMongoDbStores<ApplicationUser, MongoRole<string>, string>(options =>
        {
            options.ConnectionString = builder.Configuration.GetSection("MongoDbSettings:ConnectionString").Get<string>();
            options.UsersCollection = builder.Configuration.GetSection("MongoDbSettings:UsersCollectionName").Get<string>();
            options.RolesCollection = builder.Configuration.GetSection("MongoDbSettings:RolesCollectionName").Get<string>();
        })
        .AddDefaultTokenProviders();


        // Cookies
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/Conta/Login";
            options.AccessDeniedPath = "/Home/AccessDenied";
        });

        builder.Services.AddScoped<ProdutoService>();
        builder.Services.AddScoped<CarrinhoService>();

        // Configuração de sessão
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapStaticAssets();

        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
        app.Run();
    }
}
