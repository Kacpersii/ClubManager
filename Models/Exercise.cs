using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Exercise
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfPlayers { get; set; }
        public int ExecutionTime { get; set; }
        public string ExerciseScheme { get; set; }

    }
}