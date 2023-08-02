using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelOlymp2023.Data;
using HotelOlymp2023.Models;
using HotelOlymp2023.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using HotelOlymp2023.Interfaces;


namespace HotelOlymp2023.Controllers
{
    public class CamerasController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly AuthDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingService _bookingService;

        public CamerasController(AuthDbContext context, AuthDbContext dbContext, UserManager<ApplicationUser> userManager, IBookingService bookingService)
        {
            _context = context;
            _dbContext = dbContext;
            this._userManager = userManager;
            _bookingService = bookingService;
        }

        // GET: Cameras
        public async Task<IActionResult> Index()
        {
              return _context.Cameras != null ? 
                          View(await _context.Cameras.ToListAsync()) :
                          Problem("Entity set 'AuthDbContext.Cameras'  is null.");

        }

        // GET: Cameras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cameras == null)
            {
                return NotFound();
            }

            var camera = await _context.Cameras
                .FirstOrDefaultAsync(m => m.ID == id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        public IActionResult Index1(string CheckInDate, string CheckOutDate)
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);
            if (!string.IsNullOrEmpty(CheckInDate) && !string.IsNullOrEmpty(CheckOutDate))
            {
                
                if (DateTime.TryParse(CheckInDate, out DateTime checkInDate) &&
                    DateTime.TryParse(CheckOutDate, out DateTime checkOutDate))
                {
                   
                    var availableRooms = _dbContext.Cameras
                        .Where(room =>
                            !_dbContext.Reservations.Any(reservation =>
                                reservation.CameraId == room.ID &&
                                reservation.CheckInDate < checkOutDate &&
                                reservation.CheckOutDate > checkInDate
                            )
                        )
                        .ToList();

                    TempData["CheckInDate"] = CheckInDate;
                    TempData["CheckOutDate"] = CheckOutDate;

                    return View(availableRooms);
                }
            }

 


            var allRooms = _dbContext.Cameras.ToList();
            return View(allRooms);


        }

        [HttpPost]
        public async Task<IActionResult> BookRoom(int cameraId, DateTime checkInDate, DateTime checkOutDate)
        {
            // Assuming you have the userId available, you can get it from the current user.
            string userId = _userManager.GetUserId(this.User);

            // Use the BookingService to book the room.
            bool isBooked = await _bookingService.BookRoomAsync(cameraId, checkInDate, checkOutDate, userId);

            if (isBooked)
            {
                // Room booked successfully, do something.
                return RedirectToAction("Index1");
            }
            else
            {
                // Room booking failed, handle the error.
                TempData["ErrorMessage"] = "Unable to book the room.";
                return RedirectToAction("Index1");
            }
        }



        // GET: Cameras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cameras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,RoomNumber,RoomType,Price,Capacity")] Camera camera)
        {
            if (ModelState.IsValid)
            {
                _context.Add(camera);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camera);
        }

        // GET: Cameras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cameras == null)
            {
                return NotFound();
            }

            var camera = await _context.Cameras.FindAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return View(camera);
        }

        // POST: Cameras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoomNumber,RoomType,Price,Capacity")] Camera camera)
        {
            if (id != camera.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camera);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CameraExists(camera.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(camera);
        }

        // GET: Cameras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cameras == null)
            {
                return NotFound();
            }

            var camera = await _context.Cameras
                .FirstOrDefaultAsync(m => m.ID == id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        // POST: Cameras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cameras == null)
            {
                return Problem("Entity set 'AuthDbContext.Cameras'  is null.");
            }
            var camera = await _context.Cameras.FindAsync(id);
            if (camera != null)
            {
                _context.Cameras.Remove(camera);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CameraExists(int id)
        {
          return (_context.Cameras?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
