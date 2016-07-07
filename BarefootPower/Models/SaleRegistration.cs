using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarefootPower.Models
{
    public class SaleRegistration
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string GroupName { get; set; }

        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Membership { get; set; }

        [Range(0, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Present { get; set; }

        [Range(0, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Elec { get; set; }

        [Range(0, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public int Solar { get; set; }

        public DateTime Date { get; set; }

        public virtual Agent Agent { get; set; }
    }
}