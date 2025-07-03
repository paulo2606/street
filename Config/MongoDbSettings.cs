namespace street.Config
{
    public class MongoDbSettings
    {
        //passamos como .Empty para evitar valores nulos
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
