using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace street.Models
{
    public class Produto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } 

        [BsonElement("Nome")]
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [BsonElement("Descricao")]
        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("Preco")]
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [BsonElement("Estoque")]
        [Required(ErrorMessage = "O estoque é obrigatório.")]
        [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
        public int Estoque { get; set; }

        [BsonElement("ImageUrl")]
        public string? ImageUrl { get; set; }
    }
}