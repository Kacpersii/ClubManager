using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Coach
    {
        public int ID { get; set; }
        public int UserID { get; set; }

        public virtual User User { get; set; }
        public virtual List<Team> Teams { get; set; }
        public virtual List<string> Courses { get; set; }

    }
}