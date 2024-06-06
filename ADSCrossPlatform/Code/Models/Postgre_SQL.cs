using ADSCrossPlatform.Code.Context.ADS.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace ADSCrossPlatform.Code.Models
{
    public class Postgre_SQL
    {
        private PostgreSQLContext _context;

        public Postgre_SQL()
        {
            _context = new();
        }

        public async Task<IEnumerable<AddressDBModel>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<AddressDBModel>? GetAddressDBModel(Guid id)
        {
            var addressDBModel = await _context.Addresses.FindAsync(id);

            if (addressDBModel == null)
            {
                return null;
            }

            return addressDBModel;
        }

        public async Task<bool> PutAddressDBModel(Guid id, AddressDBModel addressDBModel)
        {
            if (id != addressDBModel.Id)
            {
                return false;
            }
            using (var context = new PostgreSQLContext())
            {
                context.Entry(addressDBModel).State = EntityState.Modified;

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressDBModelExists(id))
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            

            return true;
        }

        public async Task<bool> PostAddressDBModel(AddressDBModel addressDBModel)
        {
            using (var context = new PostgreSQLContext())
            {
                await context.Addresses.AddAsync(addressDBModel);
                await context.SaveChangesAsync();
            }           
                      
            return true;
        }

        public async Task<bool> DeleteAddressDBModel(Guid id)
        {
            var addressDBModel = await _context.Addresses.FindAsync(id);
            if (addressDBModel == null)
            {
                return false;
            }

            _context.Addresses.Remove(addressDBModel);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool AddressDBModelExists(Guid id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }
    }
}
