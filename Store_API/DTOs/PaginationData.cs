namespace Store_API.DTOs
{
    public class PaginationData<T> where T : class
    {
        public List<T> Data { get; set; }
        public static PaginationData<T> GetData(string query, int totalRow, int numsRow, int currentPage)
        {

            return null;
        }
    }
}
