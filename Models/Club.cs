using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Club
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Nazwa klubu")]
        public string Name { get; set; }

        [Display(Name = "Stadion")]
        public string Stadium { get; set; }

        [Display(Name = "Logo")]
        public string Logo { get; set; }

        public virtual List<Manager> Managers { get; set; }
        public virtual List<Team> Teams { get; set; }
        public virtual List<Coach> Coaches { get; set; }
        public virtual List<Player> Players { get; set; }
    }
}