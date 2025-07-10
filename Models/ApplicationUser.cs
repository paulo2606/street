using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace street.Models
{
    public class ApplicationUser : MongoUser<string>
    {
        public string? Nome { get; set; }

        public ApplicationUser() : base()
        {
            this.Id = Guid.NewGuid().ToString();

            Nome = string.Empty; 
        }

        public ApplicationUser(string userName, string email) : base(userName)
        {
            this.Id = Guid.NewGuid().ToString(); 

            this.Email = email;

            Nome = string.Empty;
        }
    }
}