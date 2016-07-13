using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarefootPower.Models
{
    public class SalesRegistrationExport
    {
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string REA { get; set; }
        public string GroupName { get; set; }
        public int TotalMembership { get; set; }
        public int Present { get; set; }
        public int WithElec { get; set; }
        public int WithSolar { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}