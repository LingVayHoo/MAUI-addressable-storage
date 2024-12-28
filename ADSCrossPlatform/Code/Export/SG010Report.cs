using ADSCrossPlatform.Code.Models;

namespace ADS.Code.Export
{
    public class SG010Report
    {
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public List<AddressDBModel> Data { get; set; } = new();
    }
}
