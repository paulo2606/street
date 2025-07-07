// Em ProdutosController.cs
using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO; // Para Path e File
using System.Text.Json; // Para JsonSerializer
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment

namespace street.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ProdutoService _produtoService;
        private readonly IWebHostEnvironment _env; // Certifique-se que está injetado aqui

        // Injete IWebHostEnvironment no construtor
        public ProdutosController(ProdutoService produtoService, IWebHostEnvironment env)
        {
            _produtoService = produtoService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _produtoService.GetAsync();
            return View(produtos);
        }

        // ... (Seu método DetalheProduto aqui, se tiver) ...
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

        // Mantenha este método de seeding separado ou chame-o de um inicializador
        // Exemplo de um método de seeding que pode ser chamado uma vez
        public async Task<IActionResult> SeedProducts()
        {
            // O PATH DO SEU ARQUIVO JSON
            var jsonFilePath = Path.Combine(_env.ContentRootPath, "Data", "produtos.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                return BadRequest("Arquivo JSON de produtos não encontrado em: " + jsonFilePath);
            }

            var jsonString = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            // DESERIALIZA O JSON PARA A LISTA DE PRODUTOS
            var produtosIniciais = JsonSerializer.Deserialize<List<Produto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (produtosIniciais == null || !produtosIniciais.Any())
            {
                return BadRequest("Nenhum produto encontrado no arquivo JSON ou falha na desserialização.");
            }

            int produtosAdicionados = 0;
            foreach (var produto in produtosIniciais) // AGORA produtosIniciais EXISTE!
            {
                // Este GetAsync precisa da sobrecarga no ProdutoService (Expression<Func<Produto, bool>>)
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