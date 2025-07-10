using Microsoft.AspNetCore.Mvc;
using street.Models;
using street.Services;
using street.Controllers;
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

        public async Task<IActionResult> CriarProduto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarProduto([Bind("Nome,Descricao,Preco,Estoque")] Produto produto, IFormFile? imagemProduto)
        {
            if (ModelState.IsValid)
            {
                if (imagemProduto != null && imagemProduto.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagemProduto.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagemProduto.CopyToAsync(fileStream);
                    }

                    produto.ImageUrl = "/uploads/" + uniqueFileName;
                }

                await _produtoService.CreateAsync(produto);
                return RedirectToAction(nameof(ListaProdutosCompleta));
            }
            return View(produto);
        }

        public async Task<IActionResult> EditarProduto(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProduto(string id, [Bind("Id,Nome,Descricao,Preco,Estoque")] Produto produto, IFormFile? novaImagemProduto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var produtoExistente = await _produtoService.GetAsync(id);
                if (produtoExistente == null)
                {
                    return NotFound();
                }

                if (novaImagemProduto != null && novaImagemProduto.Length > 0)
                {
                    if (!string.IsNullOrEmpty(produtoExistente.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(_env.WebRootPath, produtoExistente.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + novaImagemProduto.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await novaImagemProduto.CopyToAsync(fileStream);
                    }
                    produto.ImageUrl = "/uploads/" + uniqueFileName;
                }
                else
                {
                    produto.ImageUrl = produtoExistente.ImageUrl;
                }

                await _produtoService.UpdateAsync(id, produto);
                return RedirectToAction(nameof(ListaProdutosCompleta));
            }
            return View(produto);
        }

        [HttpPost, ActionName("DeletarProduto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarProduto(string id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarProdutoConfirmado(string id) 
        {
            var produto = await _produtoService.GetAsync(id);
            if (produto == null)
            {
                return NotFound(); 
            }

            if (!string.IsNullOrEmpty(produto.ImageUrl))
            {
                string imagePath = Path.Combine(_env.WebRootPath, produto.ImageUrl.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Erro ao deletar arquivo {imagePath}: {ex.Message}");
                    }
                }
            }
            await _produtoService.RemoveAsync(id);
            return RedirectToAction(nameof(ListaProdutosCompleta)); 
        }
    }
}