using ADSCrossPlatform.Code.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS.Code.Models
{
    public class AddressHistory
    {
        public AddressDBModel addressDBModel { get; set; } = null!;
        public string ChangedBy { get; set; } = string.Empty;
    }
}
