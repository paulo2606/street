using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace street.Models
{
    public class Produto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; }

        [BsonElement("Nome")] 
        public string Nome { get; set; }

        [BsonElement("Descricao")]
        public string Descricao { get; set; }

        [BsonElement("Preco")]
        public decimal Preco { get; set; }

        [BsonElement("Estoque")]
        public int Estoque { get; set; }

        [BsonElement("ImageUrl")] 
        public string ImageUrl { get; set; } = string.Empty; 
    }
}