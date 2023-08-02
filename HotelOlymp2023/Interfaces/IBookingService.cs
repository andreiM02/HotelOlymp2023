namespace HotelOlymp2023.Interfaces
{
    public interface IBookingService
    {
        Task<bool> BookRoomAsync(int cameraId, DateTime checkInDate, DateTime checkOutDate, string userId);


    }
}
