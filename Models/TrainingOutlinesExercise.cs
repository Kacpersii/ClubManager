using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class TrainingOutlinesExercise
    {
        public int ID { get; set; }
        public int ExerciseID { get; set; }
        public int TrainingOutlineID { get; set; }


        public virtual Exercise Exercise { get; set; }
        public virtual TrainingOutline TrainingOutline { get; set; }
    }
}