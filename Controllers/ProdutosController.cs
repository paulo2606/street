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

        public async Task<IActionResult> SeedProducts()
        {
            var jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", "produtos.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                return BadRequest("Arquivo JSON de produtos não encontrado em: " + jsonFilePath);
            }

            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var produtosIniciais = JsonSerializer.Deserialize<List<Produto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (produtosIniciais == null || !produtosIniciais.Any())
            {
                return BadRequest("Nenhum produto encontrado no arquivo JSON ou falha na desserialização.");
            }

            int produtosAdicionados = 0;
            foreach (var produto in produtosIniciais)
            {
                var existing = await _produtoService.GetAsync(p => p.Nome == produto.Nome);
                if (existing == null)
                {
                    await _produtoService.CreateAsync(produto);
                    produtosAdicionados++;
                }
            }
            return Ok($"{produtosAdicionados} produtos foram adicionados/atualizados com sucesso.");
        }
    }
}