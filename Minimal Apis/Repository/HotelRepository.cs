using Microsoft.EntityFrameworkCore;
using Minimal_Apis.Data;

namespace Minimal_Apis.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private DataContext _dataContext;
        public HotelRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Hotel>> GetHotelAsync()
        {
            return await _dataContext.Hotels.ToListAsync();
        }
        public async Task<List<Hotel>> GetHotelAsync(string name)
        {
            return await _dataContext.Hotels.Where(hotel => hotel.Name.Contains(name)).ToListAsync();
        }
        public async Task<Hotel?> GetHotelAsync(int Id)
        {
            return await _dataContext.Hotels.FindAsync(new object[] { Id });

        }
        public async Task<List<Hotel>> GetHotelAsync(Coordinate coordinate)
        {
            return await _dataContext.Hotels.Where(h =>
                h.Latittude > coordinate.latilude - 1 &&
                h.Latittude < coordinate.latilude + 1 &&
                h.Longitude > coordinate.longitude - 1 &&
                h.Longitude < coordinate.longitude + 1
            ).ToListAsync();
        }
        public async Task InsertHotelAsync(Hotel hotel)
        {
            await _dataContext.Hotels.AddAsync(hotel);

        }
        public async Task UpdateHotelAsync(Hotel hotel)
        {
            var hotelFromDb = await _dataContext.Hotels.FindAsync(new object[] { hotel.Id });
            if (hotelFromDb == null)
            {
                return;
            }
            hotelFromDb.Name = hotel.Name;
            hotelFromDb.Longitude = hotel.Longitude;
            hotelFromDb.Latittude = hotel.Latittude;
        }
        public async Task DeleteHotelAsync(int Id)
        {
            var hotelFromDb = await _dataContext.Hotels.FindAsync(new object[] { Id });
            if (hotelFromDb == null)
                return;
            _dataContext.Hotels.Remove(hotelFromDb);
        }
        public async Task SaveAsync()
        {
            await _dataContext.SaveChangesAsync();
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    _dataContext.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
