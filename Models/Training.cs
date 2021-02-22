using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Training
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public string Place { get; set; }
        public int TrainingOutlineID { get; set; }
        public int TeamID { get; set; }
        public bool IsAttendanceListChecked { get; set; }

        public virtual TrainingOutline TrainingOutline { get; set; }
        public virtual Team Team { get; set; }
        public virtual List<Attendance> AttendanceList { get; set; }
    }
}