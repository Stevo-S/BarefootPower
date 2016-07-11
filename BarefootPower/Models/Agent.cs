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
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(50)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " +
                    (string.IsNullOrEmpty(MiddleName) ? "" : (MiddleName + " "))
                    + LastName;
            }
        }
    }
}