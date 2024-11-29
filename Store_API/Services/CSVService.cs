using CsvHelper;
using Dapper;
using Microsoft.Data.SqlClient;
using Store_API.Repositories;
using System.Formats.Asn1;
using System.Globalization;

namespace Store_API.Services
{
    public class CSVService : ICSVRepository
    {
        public async Task<List<T>> ReadCSV<T>(IFormFile file)
        {
            List<T> records = new();
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<T>().AsList();
            }    
            return records;
        }
    }
}
