namespace Store_API.IService
{
    public interface ICSVService
    {
        public Task<List<T>> ReadCSV<T>(IFormFile file);
    }
}
