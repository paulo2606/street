using MongoDB.Driver;
using street.Data;
using street.Models;
using Microsoft.AspNetCore.Http;
using street.Controllers; // Não é estritamente necessário aqui, mas manter se já estava.

namespace street.Services
{
    public class CarrinhoService
    {
        private readonly IMongoCollection<street.Models.CarrinhoDeCompras> _carrinhosCollection;
        public CarrinhoService(MongoDbContext dbContext)
        {
            _carrinhosCollection = dbContext.GetCollection<street.Models.CarrinhoDeCompras>("ShoppingCart");
        }

        public async Task<List<street.Models.CarrinhoDeCompras>> GetAsync() =>
            await _carrinhosCollection.Find(_ => true).ToListAsync();

        public async Task<street.Models.CarrinhoDeCompras?> GetByIdAsync(string id) =>
            await _carrinhosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(street.Models.CarrinhoDeCompras newCarrinho) =>
            await _carrinhosCollection.InsertOneAsync(newCarrinho);

        public async Task UpdateAsync(string id, street.Models.CarrinhoDeCompras updatedCarrinho) =>
            await _carrinhosCollection.ReplaceOneAsync(x => x.Id == id, updatedCarrinho);

        public async Task RemoveAsync(string id) =>
            await _carrinhosCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<street.Models.CarrinhoDeCompras> ObterOuCriarCarrinhoAsync(HttpContext httpContext)
        {
            string carrinhoId = httpContext.Session.GetString("CarrinhoId");
            street.Models.CarrinhoDeCompras carrinho = null;

            if (!string.IsNullOrEmpty(carrinhoId))
            {
                // Tenta obter o carrinho existente.
                // Se o ID na sessão for um ObjectId válido, funcionará.
                carrinho = await GetByIdAsync(carrinhoId);
            }

            if (carrinho == null)
            {
                // Se o carrinho não existe ou o ID na sessão é inválido/não encontrado, cria um novo.
                carrinho = new street.Models.CarrinhoDeCompras
                {
                    // NÃO ATRIBUA UM ID AQUI. O MongoDB o gerará automaticamente.
                    Itens = new List<street.Models.CarrinhoDeCompras.ItemCarrinho>()
                };

                await CreateAsync(carrinho); // AQUI o MongoDB atribuirá um ObjectId ao carrinho.Id

                // Agora que o carrinho foi salvo e tem um Id do MongoDB, armazene-o na sessão.
                httpContext.Session.SetString("CarrinhoId", carrinho.Id);
            }
            else
            {
                // Se o carrinho já existia, apenas garanta que o ID está na sessão.
                httpContext.Session.SetString("CarrinhoId", carrinho.Id);
            }
            return carrinho;
        }
    }
}