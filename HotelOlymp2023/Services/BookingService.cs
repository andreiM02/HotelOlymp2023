using HotelOlymp2023.Areas.Identity.Data;
using HotelOlymp2023.Data;
using HotelOlymp2023.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelOlymp2023.Interfaces;


public class BookingService : IBookingService
{
    private readonly AuthDbContext _context;
    private readonly AuthDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingService(AuthDbContext context, AuthDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _dbContext = dbContext;
        this._userManager = userManager;
    }

    public async Task<bool> BookRoomAsync(int cameraId, DateTime checkInDate, DateTime checkOutDate, string userId)
    {
        var camera = await _context.Cameras.FindAsync(cameraId);
        if (camera == null)
        {
            return false;
        }

        if (userId == null)
        {
            return false;
        }

        var overlappingReservations = await _context.Reservations
            .Where(r => r.CameraId == cameraId &&
                        !(r.CheckOutDate <= checkInDate || r.CheckInDate >= checkOutDate)).ToListAsync();

        if (overlappingReservations.Count > 0)
        {
            return false;
        }

        Reservation reservation = new Reservation
        {
            UserId = userId,
            CameraId = camera.ID,
            CheckInDate = DateTime.Parse(checkInDate.ToString("yyyy-MM-dd")),
            CheckOutDate = DateTime.Parse(checkOutDate.ToString("yyyy-MM-dd"))
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return true;
    }
}
