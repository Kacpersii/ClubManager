using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CoachID { get; set; }

        public virtual Coach Coach { get; set; }
        public virtual List<Player> Players { get; set; }
    }
}