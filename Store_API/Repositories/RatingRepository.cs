using Store_API.Data;

namespace Store_API.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        public RatingRepository(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;
        }
        public async Task<double> GetRating(int productId)
        {
            string query = @"   SELECT IIF(ROUND(AVG(Star), 2) is NULL, 0, ROUND(AVG(Star), 2)) as Rating 
                                FROM Ratings 
                                WHERE ProductId = @ProductId ";
            var p = new { ProductId = productId };
            var rating = await _dapperService.QueryFirstOrDefaultAsync(query, p);
            return rating.Rating;
        }

        public async Task SetRating(int userId, int productId, double star)
        {
            string query = @"
                            DECLARE @IsExisted INT

                            SELECT @IsExisted = Id FROM Ratings WHERE UserId = @UserId
                            IF(@IsExisted is NULL)
	                            BEGIN
		                            INSERT INTO Ratings VALUES(@Star, @ProductId, @UserId)
	                            END
                            ELSE
	                            BEGIN
		                            UPDATE Ratings SET Star = @Star WHERE Id = @IsExisted
	                            END
                            ";
            var p = new { ProductId = productId, UserId = userId, Star = star };

            try
            {
                await _dapperService.Execute(query, p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
