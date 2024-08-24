using ADSCrossPlatform.Code.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSCrossPlatform.Code.Interfaces
{
    public interface IDataBase
    {
        //public IEnumerable<AddressDBModel>? GetAllContent();
        public Task<IEnumerable<AddressDBModel>?> GetContentByArticle(string article);
        public Task<string> GetDataFromMyWarehouseByArt(string article);
        public Task<string> GetDataFromMyWarehouseByName(string article);
        public Task<bool> PutAddressDBModel(Guid id, AddressDBModel addressDBModel);
        public Task<bool> PostAddressDBModel(AddressDBModel addressDBModel);
        public Task<bool> DeleteAddressDBModel(Guid id);
    }
}
