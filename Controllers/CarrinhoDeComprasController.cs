using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Conventions;
using street.Controllers;
using street.Models;
using street.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        [Authorize]
        public async Task<IActionResult> MeuCarrinho()
        {
            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Não foi possível carregar ou criar o carrinho.";
                return RedirectToAction("Index", "Home");
            }

            return View(carrinho);
        }

        [Authorize]
        public async Task<IActionResult> CriarOuRedirecionarParaCarrinho()
        {
            await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);
            return RedirectToAction("MeuCarrinho");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarItem(string produtoId, int quantidade)
        {
            if (quantidade <= 0)
            {
                TempData["ErrorMessage"] = "A quantidade deve ser maior que zero.";
                return RedirectToAction("MeuCarrinho");
            }

            var produto = await _produtoService.GetAsync(produtoId);

            if (produto == null)
            {
                TempData["ErrorMessage"] = "Produto não encontrado.";
                return RedirectToAction("MeuCarrinho");
            }

            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Não foi possível carregar ou criar o carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            int quantidadeNoCarrinhoAtual = itemExistente?.Quantidade ?? 0;
            int quantidadeTotalDesejada = quantidadeNoCarrinhoAtual + quantidade;

            if (produto.Estoque < quantidadeTotalDesejada)
            {
                TempData["ErrorMessage"] = $"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.Estoque}. Você já tem {quantidadeNoCarrinhoAtual} no carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

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

            produto.Estoque -= quantidade;
            await _produtoService.UpdateAsync(produto.Id, produto);

            TempData["SuccessMessage"] = $"Produto '{produto.Nome}' adicionado ao carrinho!";
            return RedirectToAction("MeuCarrinho");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarQuantidade(string itemId, int novaQuantidade)
        {
            if (novaQuantidade < 0)
            {
                TempData["ErrorMessage"] = "A quantidade não pode ser negativa.";
                return RedirectToAction("MeuCarrinho");
            }

            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);
            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Não foi possível carregar o carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.Id == itemId);
            if (itemExistente == null)
            {
                TempData["ErrorMessage"] = "Item não encontrado no carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

            var produto = await _produtoService.GetAsync(itemExistente.ProdutoId);
            if (produto == null)
            {
                TempData["ErrorMessage"] = "Produto associado ao item não encontrado.";
                carrinho.Itens.Remove(itemExistente);
                await _carrinhoService.UpdateAsync(carrinho.Id, carrinho);
                return RedirectToAction("MeuCarrinho");
            }

            int quantidadeAnterior = itemExistente.Quantidade;

            if (novaQuantidade == 0)
            {
                carrinho.Itens.Remove(itemExistente);

                produto.Estoque += quantidadeAnterior;
                await _produtoService.UpdateAsync(produto.Id, produto);

                carrinho.DataUltimaAtualizacao = DateTime.UtcNow;
                await _carrinhoService.UpdateAsync(carrinho.Id, carrinho);
                TempData["SuccessMessage"] = $"Produto '{itemExistente.NomeProduto}' removido do carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

            int diferencaQuantidade = novaQuantidade - quantidadeAnterior;

            if (diferencaQuantidade > 0)
            {
                if (produto.Estoque < diferencaQuantidade)
                {
                    TempData["ErrorMessage"] = $"Estoque insuficiente para adicionar mais '{diferencaQuantidade}' unidades de '{produto.Nome}'. Disponível: {produto.Estoque}.";
                    return RedirectToAction("MeuCarrinho");
                }
                produto.Estoque -= diferencaQuantidade;
            }
            else if (diferencaQuantidade < 0)
            {
                produto.Estoque -= diferencaQuantidade;
            }

            itemExistente.Quantidade = novaQuantidade;
            carrinho.DataUltimaAtualizacao = DateTime.UtcNow;
            await _carrinhoService.UpdateAsync(carrinho.Id, carrinho);
            await _produtoService.UpdateAsync(produto.Id, produto);

            TempData["SuccessMessage"] = $"Quantidade de '{itemExistente.NomeProduto}' atualizada para {novaQuantidade}.";
            return RedirectToAction("MeuCarrinho");
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                TempData["ErrorMessage"] = "ID do item inválido para remoção.";
                return RedirectToAction("MeuCarrinho");
            }

            var carrinho = await _carrinhoService.ObterOuCriarCarrinhoAsync(HttpContext);
            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Não foi possível carregar o carrinho.";
                return RedirectToAction("MeuCarrinho");
            }

            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.Id == itemId);

            if (itemExistente != null)
            {
                var produto = await _produtoService.GetAsync(itemExistente.ProdutoId);
                if (produto != null)
                {
                    produto.Estoque += itemExistente.Quantidade;
                    await _produtoService.UpdateAsync(produto.Id, produto);
                }
                else
                {
                    Console.WriteLine($"Aviso: Produto com ID {itemExistente.ProdutoId} não encontrado ao remover item do carrinho.");
                }

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