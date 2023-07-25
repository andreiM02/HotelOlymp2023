using System.ComponentModel.DataAnnotations;

namespace HotelOlymp2023.Models
{
    public class Camera
    {
        public int ID { get; set; }


        public string RoomNumber { get; set; }

        public string RoomType { get; set; }


        [DataType(DataType.Currency)]
        public decimal Price { get; set; }


        public int Capacity { get; set; }
    }
}
