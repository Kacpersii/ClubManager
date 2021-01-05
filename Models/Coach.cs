using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClubManager.Models
{
    public class Coach
    {
        public int ID { get; set; }

        [Display(Name = "Nazwa użytkownika")]
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string UserName { get; set; }

        [Display(Name = "Imię")]
        [Required]
        [StringLength(25, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required]
        [StringLength(25, MinimumLength = 1)]
        public string LastName { get; set; }

        [Display(Name = "Data urodzenia")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Numer telefonu")]
        [Required]
        [StringLength(9, MinimumLength = 9)]
        public string PhoneNumber { get; set; }
        public int ClubID { get; set; }


        public virtual Club Club { get; set; }
        public virtual List<Team> Teams { get; set; }

    }
}