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
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // config mongo e identity
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


        // cookies para autenticação
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

        // config p sessão
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

        //config para adicionar o usuário admin e a role Admin
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<MongoRole<string>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminRoleName = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new MongoRole<string>(adminRoleName)); 
                Console.WriteLine($"Role '{adminRoleName}' criada com sucesso.");
            }

            const string adminEmail = "admin@smoda.com";
            const string adminPassword = "admin@smoda.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    Console.WriteLine($"Usuário admin '{adminEmail}' criado com sucesso e adicionado à role '{adminRoleName}'.");
                }
                else
                {
                    Console.WriteLine($"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"Usuário admin '{adminEmail}' já existe.");
                if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                    Console.WriteLine($"Usuário admin '{adminEmail}' adicionado à role '{adminRoleName}'.");
                }
            }
        }

        app.Run();
    }
}