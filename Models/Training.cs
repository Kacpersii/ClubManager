using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Training
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public int TrainingOutlineID { get; set; }
        public int TeamID { get; set; }

        public virtual TrainingOutline TrainingOutline { get; set; }
        public virtual Team Team { get; set; }
        public virtual List<Player> AttendanceList { get; set; }
    }
}