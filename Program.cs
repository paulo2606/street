using street.Config;
using street.Data;

var builder = WebApplication.CreateBuilder(args);

//config para o mongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<street.Services.ProdutoService>();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
