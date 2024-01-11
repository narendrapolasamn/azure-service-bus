namespace Car.Infrastructure.Configuration.Interfaces
{
    public interface ISqlDbDataServiceConfiguration
    {
        string ConnectionString { get; set; }
    }
}
