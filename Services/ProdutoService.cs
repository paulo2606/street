using MongoDB.Driver;
using street.Data;
using street.Models;
using System.Linq.Expressions;

namespace street.Services
{
    public class ProdutoService
    {
        private readonly IMongoCollection<Produto> _produtosCollection;

        public ProdutoService(MongoDbContext dbContext)
        {
            _produtosCollection = dbContext.GetCollection<Produto>("Products");
        }

        public async Task<List<Produto>> GetAsync() =>
            await _produtosCollection.Find(_ => true).ToListAsync();

        public async Task<Produto?> GetAsync(string id) =>
            await _produtosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Produto?> GetAsync(Expression<Func<Produto, bool>> filterExpression) =>
            await _produtosCollection.Find(filterExpression).FirstOrDefaultAsync();

        public async Task CreateAsync(Produto newProduto) =>
            await _produtosCollection.InsertOneAsync(newProduto);

        public async Task UpdateAsync(string id, Produto updatedProduto) =>
            await _produtosCollection.ReplaceOneAsync(x => x.Id == id, updatedProduto);

        public async Task RemoveAsync(string id) =>
            await _produtosCollection.DeleteOneAsync(x => x.Id == id);
    }
}