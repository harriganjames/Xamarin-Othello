namespace Aub.Xamarin.Toolkit.Service
{
    public interface IPersistenceService
    {
        string Get(string key);
        T Get<T>(string key);
        void Save(string key, string data);
        void Save<T>(string key, T data);
        void Remove(string key);
    }
}