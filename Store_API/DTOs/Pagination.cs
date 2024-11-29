using Store_API.DTOs.Products;
using Store_API.Helpers;

namespace Store_API.DTOs
{
    public class Pagination<T> where T : class
    {
        public List<T> DataInCurrentPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalRow { get; set; }
        public int RowInPage { get; set; }

        public static Pagination<T> GetPaginationData(List<T> source, int total_row, int currentPage, int count_in_page)
        {
            if (source == null || source.Count == 0) return null;

            var dataPagination = new Pagination<T>()
            {
                DataInCurrentPage = source,
                CurrentPage = currentPage,
                RowInPage = CF.GetInt(source.Count),
                TotalRow = total_row,
                TotalPage = total_row % count_in_page == 0 ? total_row / count_in_page : (total_row / count_in_page) + 1,
            };

            return dataPagination;
        }
    }
}
