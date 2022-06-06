using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Room
    {
        public int RoomId { get; set; }

        [Display(Name = "Room Name")]
        public string Name { get; set; }

        [Display(Name = "Room Capacity")]
        public int Capacity { get; set; }

        public string Notes { get; set; }

        public virtual List<Timetable> Events { get; set; }
    }
}
