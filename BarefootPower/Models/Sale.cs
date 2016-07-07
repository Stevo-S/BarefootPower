using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BarefootPower.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Index("IX_ClientAndGroup", 1, IsUnique = true)]
        [StringLength(100)]
        public string ClientName { get; set; }

        [StringLength(20)]
        public string ClientPhone { get; set; }

        public int C600 { get; set; }
        public int C3000 { get; set; }
        public int C3000TV { get; set; }
        
        public virtual SaleRegistration SaleRegistration { get; set; }
    }
}