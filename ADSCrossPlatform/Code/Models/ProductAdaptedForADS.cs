using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSCrossPlatform.Code.Models
{
    public class ProductAdaptedForADS
    {
        public string ID { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string[]? Storages { get; set; }
        public double[]? QtyInStorages { get; set; }
    }
}
