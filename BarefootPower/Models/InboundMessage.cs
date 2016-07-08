using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BarefootPower.Models;

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
            catch (MissingFieldException ex)
            {
                response = ex.Message;
            }
            catch (FormatException ex)
            {
                response = ex.Message;
            }
            catch (ValidationException ex)
            {
                response = ex.Message;
            }
            catch (Exception ex)
            {
                response = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
            string[] sentSales;

            if (string.IsNullOrEmpty(Message))
            {
                throw new NullReferenceException("Message cannot be null or empty");
            }

            fields = Message.Split('*');
            if (fields.Length < expectedNumberOfFields)
            {
                throw new MissingFieldException("The number of fields is less than expected");
            }

            groupName = fields[0].Trim();

            if (!Int32.TryParse(fields[1], out membership))
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

            sentSales = fields[5].Split(',');
            if (sentSales.Length < solar)
            {
                throw new MissingFieldException("The number of sales is less than the expected " + sentSales.Length.ToString());
            }
           
            using (var db = new ApplicationDbContext())
            {
                var agent = db.Agents.Where(a => a.Phone.Contains(Sender)).FirstOrDefault();
                if (agent == null)
                {
                    throw new ValidationException("Agent is not registered");
                }

                var saleRegistration = new SaleRegistration()
                {
                    Agent = agent,
                    GroupName = groupName,
                    Membership = membership,
                    Present = present,
                    Elec = elec,
                    Solar = solar,
                    Date = DateTime.Now
                };

                db.SaleRegistrations.Add(saleRegistration);
                foreach (var sentSale in sentSales)
                {
                    int numberOfSaleFields = 3;
                    string[] saleFields = sentSale.Split('#');
                    int c600 = 0, c3000 = 0, c3000tv = 0;

                    if (saleFields.Length != numberOfSaleFields)
                    {
                        throw new MissingFieldException("The number of fields is less than expected");
                    }

                    switch (saleFields[2].Trim().ToLower())
                    {
                        case "c600":
                            c600 = 1;
                            break;

                        case "c3000":
                            c3000 = 1;
                            break;

                        case "c3000tv":
                            c3000tv = 1;
                            break;

                        default:
                            throw new ValidationException("Could not read product: Product should be 'C600', 'C3000' or 'C3000TV'");
                    }

                    var sale = new Sale()
                    {
                        ClientName = saleFields[0].Trim(),
                        ClientPhone = saleFields[1].Trim(),
                        SaleRegistration = saleRegistration,
                        C600 = c600,
                        C3000 = c3000,
                        C3000TV = c3000tv
                    };

                    db.Sales.Add(sale);

                }
                db.SaveChanges();
            }

            return "Successfully recorded the sales";


        }
    }
}