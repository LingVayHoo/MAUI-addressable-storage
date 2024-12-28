using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS.Code.Export
{
    public class Allocation
    {
        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double StockFirst { get; set; }
        public double StockSecond { get; set; }
    }
}
