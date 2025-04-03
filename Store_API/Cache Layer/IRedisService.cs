namespace Store_API.Cache_Layer
{
    public interface IRedisService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
    }
}
