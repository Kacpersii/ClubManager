﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Player
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ClubID { get; set; }
        public int TeamID { get; set; }
        public LeadingLeg LeadingLeg { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int ShirtsNumber { get; set; }
        public string MainPosition { get; set; }
        public string SecondPosition { get; set; }

        public virtual User User { get; set; }
        public virtual Club Club { get; set; }
        public virtual Team Team { get; set; }

    }

    public enum LeadingLeg
    {
        [Display(Name = "Lewa")]
        Left,
        [Display(Name = "Prawa")]
        Right,
        [Display(Name = "Obie")]
        Both
    }

}