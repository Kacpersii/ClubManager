using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Manager
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ClubID { get; set; }

        public virtual User User { get; set; }
        public virtual Club Club { get; set; }
    }
}