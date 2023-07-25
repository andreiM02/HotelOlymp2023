using System;

namespace HotelOlymp2023.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string UserId { get; set; } // ID-ul utilizatorului care a efectuat rezervarea
        public int CameraId { get; set; } // ID-ul camerei care a fost rezervată
        public DateTime CheckInDate { get; set; } // Data de check-in pentru rezervare
        public DateTime CheckOutDate { get; set; } // Data de check-out pentru rezervare

        // Proprietăți adiționale, dacă este nevoie
    }
}
