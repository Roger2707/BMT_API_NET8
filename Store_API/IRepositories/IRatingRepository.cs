namespace Store_API.Repositories
{
    public interface IRatingRepository
    {
        public Task<double> GetRating(int productId);
        public Task SetRating(int userId, int productId, double star);
    }
}
