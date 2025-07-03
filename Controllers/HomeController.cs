using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using street.Data;
using street.Models;

namespace street.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, MongoDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            try
            {
                var testConection = _dbContext.GetCollection<BsonDocument>("Produto");

                var document = new BsonDocument
                {
                    { "Name", "Test Product" },
                    { "Price", 19.99 },
                    { "Category", "Test Category" }
                };

                testConection.InsertOne(document);
                ViewBag.Message = "Conexão com MongoDB estabelecida e um documento inserido com sucesso!";
            }
            catch (MongoConnectionException ex)
            {
                ViewBag.Message = "Erro ao conectar ao MongoDB: " + ex.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Ocorreu um erro: " + ex.Message;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
