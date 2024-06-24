using System.Threading.Tasks;

public interface IDataService
{
    Task<string> GetAsync(string endpoint);
}
