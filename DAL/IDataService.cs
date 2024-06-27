using System.Threading.Tasks;

public interface IDataService
{
    Task<string> GetAsync(string endpoint);
    Task<string> PostAsync(string endpoint, string jsonContent);
    Task<string> PutAsync(string endpoint, string jsonContent);
    Task<string> DeleteAsync(string endpoint);

}
