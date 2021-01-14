using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Club
    {
        public int ID { get; set; }
        public int ManagerID { get; set; }
        public string Name { get; set; }
        public string Stadium { get; set; }
        public string Logo { get; set; }

        public virtual User Manager { get; set; }
        public virtual List<Team> Teams { get; set; }
        public virtual List<Coach> Coaches { get; set; }
        public virtual List<Player> Players { get; set; }
    }
}