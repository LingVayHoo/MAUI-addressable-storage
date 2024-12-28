using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSCrossPlatform.Code.Models
{
    public class ProductForMove
    {
        public string Id { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public string OrderedQty { get; set; } = string.Empty;
    }

    public class ProductsWithStores
    {
        public List<ProductForMove>? ProductsForMove;
        public string[]? stores;
        public string CreatorName = string.Empty;
    }
}
