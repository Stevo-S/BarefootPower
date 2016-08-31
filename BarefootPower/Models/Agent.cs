using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarefootPower.Models
{
    public class Agent
    {
        public int Id { get; set; }

        [StringLength(50)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
        [Display(Name = "Branch")]
        [StringLength(100)]
        public string Location { get; set; }

        [DisplayName("Active?")]
        public bool isActive { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}