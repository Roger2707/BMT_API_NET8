namespace Store_API.Repositories
{
    public interface IRatingRepository
    {
        public Task<double> GetRating(Guid productId);
        public Task SetRating(int userId, Guid productId, double star);
    }
}
