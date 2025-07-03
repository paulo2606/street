using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace street.Models
{
    public class WebMVCMongoDB
    {   
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public int? Idade { get; set; }
        public string? Email { get; set; }
        public Array? Interesses { get; set; }


    }
}
