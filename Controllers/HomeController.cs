using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using System.Threading.Tasks; 

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}