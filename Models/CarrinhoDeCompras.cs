using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace street.Models
{
    public class CarrinhoDeCompras
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Status { get; set; } = "Ativo";
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;

        public List<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();

        public class ItemCarrinho
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();

            public string ProdutoId { get; set; }
            public string NomeProduto { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
            public decimal Total => Quantidade * PrecoUnitario;
            public string ImageUrl { get; set; }
        }
    }
}