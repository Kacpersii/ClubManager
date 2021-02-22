using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Attendance
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int TrainingID { get; set; }
        public bool WasPresent { get; set; }

        public virtual Player Player { get; set; }
        public virtual Training Training { get; set; }
    }
}