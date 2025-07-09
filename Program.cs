using street.Config;
using street.Data;
using street.Services;

var builder = WebApplication.CreateBuilder(args);

//config para o mongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<street.Services.ProdutoService>();
builder.Services.AddScoped<street.Services.CarrinhoService>();
builder.Services.AddSingleton<CarrinhoService>();
builder.Services.AddSingleton<MongoDbContext>();

// Configurações de sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

// Add services to the container.
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
app.UseSession(); // Isso DEVE ser colocado DEPOIS de UseRouting() e ANTES de UseAuthorization()

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
