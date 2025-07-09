using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO; 
using System.Text.Json;
using Microsoft.AspNetCore.Hosting; 

namespace street.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ProdutoService _produtoService;
        private readonly IWebHostEnvironment _env; 

        public ProdutosController(ProdutoService produtoService, IWebHostEnvironment env)
        {
            _produtoService = produtoService;
            _env = env;
        }

        public async Task<IActionResult> ListaProdutosCompleta()
        {
            var produtos = await _produtoService.GetAsync();
            return View(produtos);
        }

        public async Task<IActionResult> DetalheProduto(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _produtoService.GetAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);
        }

        public async Task<IActionResult> CarrinhoDeCompras()
        {
            return View();
        }
    }
}