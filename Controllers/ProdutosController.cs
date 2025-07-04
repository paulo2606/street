using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace street.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        public async Task InitializeProducts()
        {
            var produtosIniciais = new List<Produto>
            {
                new Produto { Nome = "Camiseta OverSized Verde", Preco = 119.90m, Descricao = "Descrição da camiseta verde.", ImageUrl = "/img/camiseta_verde.jpg", Estoque = 50 },
                new Produto { Nome = "Calça Cargo Verde Com Bolso", Preco = 229.90m, Descricao = "Calça cargo moderna.", ImageUrl = "/img/calca_cargo.jpg", Estoque = 30 },
                new Produto { Nome = "Moletom Azul com Bolso", Preco = 445.90m, Descricao = "Moletom confortável.", ImageUrl = "/img/moletom_azul.jpg", Estoque = 25 },
            };

            foreach (var produto in produtosIniciais)
            {
                var existing = await _produtoService.GetAsync(p => p.Nome == produto.Nome);
                if (existing == null)
                {
                    await _produtoService.CreateAsync(produto);
                }
            }
        }
    }
}