using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using System.Threading.Tasks; // Para async/await

namespace street.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProdutoService _produtoService;

        public HomeController(ILogger<HomeController> logger, ProdutoService produtoService)
        {
            _logger = logger;
            _produtoService = produtoService;
        }
            
        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoService.GetAsync();
            return View(produtos);
        }

        // ADICIONE ESTA ACTION AQUI:
        public async Task<IActionResult> DetalheProduto(string id) // Certifique-se de que o tipo do ID (string/int) é o mesmo do seu modelo/serviço
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _produtoService.GetAsync(id); // Use o método correto do seu ProdutoService para buscar por ID
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
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