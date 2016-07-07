using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BarefootPower.Models
{
    public class InboundMessage
    {
        public int Id { get; set; }

        public string Message { get; set; }

        [StringLength(20)]
        public string Sender { get; set; }

        [StringLength(50)]
        public string ServiceId { get; set; }

        [StringLength(100)]
        public string LinkId { get; set; }

        [StringLength(100)]
        public string TraceUniqueId { get; set; }

        [StringLength(50)]
        public string Correlator { get; set; }

        [StringLength(6)]
        public string ShortCode { get; set; }

        public DateTime Timestamp { get; set; }

        public string GetResponse()
        {
            string response;

            try
            {
                response = processMessage();
            }
            catch (NullReferenceException ex)
            {
                response = ex.Message;
            }
            catch(MissingFieldException ex)
            {
                response = ex.Message;
            }
            catch(FormatException ex)
            {
                response = ex.Message;
            }

            return response;
        }

        private string processMessage()
        {
            int expectedNumberOfFields = 6;
            string[] fields;
            string groupName;
            int membership;
            int present;
            int elec;
            int solar;
            string[] sales;

            if (string.IsNullOrEmpty(Message))
            {
                throw new NullReferenceException("Message cannot be null or empty");
            }

            fields = Message.Split('*');
            if (fields.Length < expectedNumberOfFields)
            {
                throw new MissingFieldException("The number of fields is less than expected");
            }

            groupName = fields[0];

            if (! Int32.TryParse(fields[1], out membership))
            {
                throw new FormatException("Unable to read the total number of members");
            }

            if (!Int32.TryParse(fields[2], out present))
            {
                throw new FormatException("Unable to read the number of members present");
            }

            if (!Int32.TryParse(fields[3], out elec))
            {
                throw new FormatException("Unable to read the 'elec' field");
            }

            if (!Int32.TryParse(fields[4], out solar))
            {
                throw new FormatException("Unable to read the 'solar' field");
            }

            sales = fields[5].Split(',');
            if (sales.Length < solar)
            {
                throw new MissingFieldException("The number of sales is less than the expected " + sales.Length.ToString());
            }

            string parsedSales = "";
            foreach (var sale in sales)
            {
                parsedSales += sale + "\n";
            }

            return "Group: " + groupName +
                    "\nMembers: " + membership +
                    "\nPresent: " + present +
                    "\nElec: " + elec +
                    "\nSolar: " + solar +
                    "Sales: " + parsedSales 
                ;
        }
    }
}