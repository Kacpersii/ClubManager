using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class TrainingOutline
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public int AuthorID { get; set; }

        public virtual User Author { get; set; }
        public virtual List<TrainingOutlinesExercise> TrainingOutlinesExercises { get; set; }
    }
}