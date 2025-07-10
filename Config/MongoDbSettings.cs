namespace street.Config
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = string.Empty; 
        public string RolesCollectionName { get; set; } = string.Empty;
    }
}
