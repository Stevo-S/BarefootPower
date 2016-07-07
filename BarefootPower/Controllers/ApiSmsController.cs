using BarefootPower.Models;
using BarefootPower.SMSCentre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace BarefootPower.Controllers
{
    public class ApiSmsController : ApiController
    {
        [Route("api/ReceiveSMS")]
        [HttpPost]
        public IHttpActionResult ReceiveSMS()
        {
            XNamespace ns1 = ConnectionDetails.SOAPRequestNamespaces["ns1"];
            XNamespace loc = "http://www.csapi.org/schema/parlayx/sms/notification/v2_2/local";
            XNamespace v2 = ConnectionDetails.SOAPRequestNamespaces["v2"];

            string notificationSoapString = Request.Content.ReadAsStringAsync().Result;
            //Elmah.ErrorSignal.FromCurrentContext().Raise(new InvalidOperationException("Unable to process received SMS:" + notificationSoapString));
            XElement soapEnvelope = XElement.Parse(notificationSoapString);

            string sender = (string)
                                    (from el in soapEnvelope.Descendants("senderAddress")
                                     select el).First();
            sender = sender.Substring(4);


            string message = (string)
                                        (from el in soapEnvelope.Descendants("message")
                                         select el).First();

            string serviceId = (string)
                                    (from el in soapEnvelope.Descendants(ns1 + "serviceId")
                                     select el).First();

            string shortCode = (string)
                                    (from el in soapEnvelope.Descendants("smsServiceActivationNumber")
                                     select el).First();
            shortCode = shortCode.Substring(4);

            string correlator = (string)
                                (from el in soapEnvelope.Descendants(loc + "correlator")
                                 select el).First();

            string linkId = (string)
                                (from el in soapEnvelope.Descendants(ns1 + "linkid")
                                 select el).First();

            string traceUniqueId = (string)
                                        (from el in soapEnvelope.Descendants(ns1 + "traceUniqueID")
                                         select el).First();

            using (var db = new ApplicationDbContext())
            {

                var incoming = new InboundMessage()
                {
                    Sender = sender,
                    Correlator = correlator,
                    LinkId = linkId,
                    Message = message,
                    ServiceId = serviceId,
                    ShortCode = shortCode,
                    Timestamp = DateTime.Now,
                    TraceUniqueId = traceUniqueId
                };


                db.InboundMessages.Add(incoming);
                db.SaveChanges();

                var outgoing = new OutboundMessage()
                {
                    Destination = incoming.Sender,
                    LinkId = incoming.LinkId,
                    Message = incoming.GetResponse(),
                    ServiceId = serviceId,
                    Correlator = incoming.Correlator
                };

                db.OutboundMessages.Add(outgoing);
                db.SaveChanges();

                
                try
                {
                    outgoing.Send();
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            string response = "";/*@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:loc=""http://www.csapi.org/schema/parlayx/sms/notification/v2_2/local"">    
                                        <soapenv:Header/>    
                                        <soapenv:Body>       
                                            <loc:notifySmsReceptionResponse/>    
                                        </soapenv:Body> 
                                    </soapenv:Envelope> "; */
            return Ok(response);

        }

        [Route("api/ReceiveDeliveryNotification")]
        [HttpPost]
        public IHttpActionResult ReceiveDeliveryNotification()
        {
            XNamespace ns1 = ConnectionDetails.SOAPRequestNamespaces["ns1"];
            XNamespace ns2 = ConnectionDetails.SOAPRequestNamespaces["ns2"];
            XNamespace loc = ConnectionDetails.SOAPRequestNamespaces["loc"];
            XNamespace v2 = ConnectionDetails.SOAPRequestNamespaces["v2"];

            string notificationSoapString = Request.Content.ReadAsStringAsync().Result;
            XElement soapEnvelope = XElement.Parse(notificationSoapString);

            string destination = (string)
                                    (from el in soapEnvelope.Descendants("address")
                                     select el).First();
            destination = destination.Substring(4);


            string deliveryStatus = (string)
                                        (from el in soapEnvelope.Descendants("deliveryStatus")
                                         select el).First();

            string serviceId = (string)
                                    (from el in soapEnvelope.Descendants(ns1 + "serviceId")
                                     select el).First();

            string correlator = (string)
                                (from el in soapEnvelope.Descendants(ns2 + "correlator")
                                 select el).First();

            string traceUniqueId = (string)
                                        (from el in soapEnvelope.Descendants(ns1 + "traceUniqueID")
                                         select el).First();

            using (var db = new ApplicationDbContext())
            {

                var deliveryNotification = new Delivery()
                {
                    Destination = destination,
                    DeliveryStatus = deliveryStatus,
                    ServiceId = serviceId,
                    Correlator = correlator,
                    TimeStamp = DateTime.Now,
                    TraceUniqueId = traceUniqueId
                };

                db.Deliveries.Add(deliveryNotification);
                db.SaveChanges();

                //return CreatedAtRoute("DefaultApi", new { id = deliveryNotification.Id }, deliveryNotification);
                return Ok("");
            }

        }
    }
}
