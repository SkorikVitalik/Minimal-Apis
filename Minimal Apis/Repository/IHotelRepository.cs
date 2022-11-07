using Microsoft.EntityFrameworkCore;

namespace Minimal_Apis.Repository
{
    public interface IHotelRepository: IDisposable
    {
        public Task<List<Hotel>> GetHotelAsync();
        public Task<Hotel?> GetHotelAsync(int Id);
        public Task<List<Hotel>> GetHotelAsync(Coordinate coordinate);
        public Task<List<Hotel>> GetHotelAsync(string name);
        public Task InsertHotelAsync(Hotel hotel);
        public Task UpdateHotelAsync(Hotel hotel);
        public Task DeleteHotelAsync(int id);
        public Task SaveAsync();

    }
}
