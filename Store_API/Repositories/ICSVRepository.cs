namespace Store_API.Repositories
{
    public interface ICSVRepository
    {
        public Task<List<T>> ReadCSV<T>(IFormFile file);
    }
}
