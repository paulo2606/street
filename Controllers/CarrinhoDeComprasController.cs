// CarrinhoDeComprasController.cs
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Conventions;
using street.Models;
using street.Services;
using System.Globalization;

namespace street.Controllers
{
    public class CarrinhoDeCompras : Controller
    {
        private readonly CarrinhoService _carrinhoService;
        private readonly ProdutoService _produtoService;

        public CarrinhoDeCompras(CarrinhoService carrinhoService, ProdutoService produtoService)
        {
            _carrinhoService = carrinhoService;
            _produtoService = produtoService;
        }

        public async Task<IActionResult> MeuCarrinho() 
        {
            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);

            if (carrinho == null)
            {
                return NotFound("Carrinho não encontrado ou não foi possível criar um novo carrinho.");
            }

            return View(carrinho);
        }

        public async Task<IActionResult> CriarOuRedirecionarParaCarrinho()
        {
            await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);

            return RedirectToAction("MeuCarrinho");
        }

        public async Task<IActionResult> AdicionarItem(string produtoId, int quantidade)
        {
            var produto = await _produtoService.GetAsync(produtoId);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }


            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);

            if (produto.Estoque < quantidade) 
            {
                TempData["ErrorMessage"] = $"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.Estoque}";
                return RedirectToAction("MeuCarrinho");
            }


            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);

            string nome = produto.Nome;
            decimal precoUnitario = produto.Preco;
            string imagemUrl = produto.ImageUrl;

            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                var novoItem = new street.Models.CarrinhoDeCompras.ItemCarrinho
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    NomeProduto = nome,
                    PrecoUnitario = precoUnitario,
                    ImageUrl = imagemUrl
                };
                carrinho.Itens.Add(novoItem);
            }
            carrinho.DataUltimaAtualizacao = DateTime.UtcNow; 
            await _carrinhoService.UpdateAsync(carrinho.Id, carrinho);

            TempData["SuccessMessage"] = $"Produto '{produto.Nome}' adicionado ao carrinho!"; 
            return RedirectToAction("MeuCarrinho");
        }

        [HttpPost]
        public async Task<IActionResult> RemoverItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                TempData["ErrorMessage"] = "ID do item inválido para remoção.";
                return RedirectToAction("MeuCarrinho");
            }

            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);
            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.Id == itemId);

            if (itemExistente != null)
            {
                carrinho.Itens.Remove(itemExistente);   
                carrinho.DataUltimaAtualizacao = DateTime.UtcNow; 
                await _carrinhoService.UpdateAsync(carrinho.Id, carrinho);
                TempData["SuccessMessage"] = "Item removido do carrinho com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Item não encontrado no carrinho.";
            }
            return RedirectToAction("MeuCarrinho");
        }
    }
}