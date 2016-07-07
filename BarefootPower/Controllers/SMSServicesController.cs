using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using BarefootPower.SMSCentre;
using System.Net.Http;
using System.Net;
using System.Text;
using BarefootPower.Models;

namespace BarefootPower.Controllers
{
    public class SMSServicesController : Controller
    {
        private static XNamespace loc = "http://www.csapi.org/schema/parlayx/sms/notification_manager/v2_3/local";
        // GET: SMSServices
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult StartSMSNotification()
        {
            
            return View();
        }
        
        public ActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Send(string shortCode, string serviceId, string message, string destination)
        {
            using(var db = new ApplicationDbContext())
            {
                var outgoing = new OutboundMessage()
                {
                    ServiceId = serviceId,
                    Message = message,
                    Destination = destination,
                    LinkId = "123456789"
                };

                db.OutboundMessages.Add(outgoing);
                db.SaveChanges();
                TempData["SendSmsResponse"] = outgoing.Send();
            }
            return View();
        }

        [HttpPost]
        public ActionResult StartSMSNotification(string shortCode, string serviceId, string criteria, string correlator)
        {
            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            XNamespace soapenv = ConnectionDetails.SOAPRequestNamespaces["soapenv"];
            XNamespace v2 = ConnectionDetails.SOAPRequestNamespaces["v2"];
            //XNamespace loc = //ConnectionDetails.SOAPRequestNamespaces["loc"];
            XElement soapEnvelope =
                new XElement(soapenv + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "v2", v2.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "loc", loc.NamespaceName),
                    new XElement(soapenv + "Header",
                            new XElement(v2 + "RequestSOAPHeader",
                                new XAttribute("xmlns", v2.NamespaceName),
                                new XElement(v2 + "spId", ConnectionDetails.GetSpID()),
                                new XElement(v2 + "spPassword", ConnectionDetails.HashPassword(ConnectionDetails.GetSpID() + ConnectionDetails.GetPassword() + timestampString)),
                                new XElement(v2 + "serviceId", serviceId),
                                new XElement(v2 + "timeStamp", timestampString)
                            )
                    ),
                    new XElement(soapenv + "Body",
                        new XElement(loc + "startSmsNotification",
                            new XElement(loc + "reference",
                                new XElement("endpoint", "http://" + ConnectionDetails.GetHostPPPAddress() + "/BarefootPower/api/ReceiveSMS"),
                                new XElement("interfaceName", "notifySmsReception"),
                                new XElement("correlator", correlator)
                            ),
                            new XElement(loc + "smsServiceActivationNumber", shortCode)
                        )
                    )
                );

            var notificationRequest = soapEnvelope.ToString();
            TempData["SmsNotificationResponse"] = Send(notificationRequest, timestampString);
            TempData["SmsNotificationRequest"] = notificationRequest;
            return View("StopSMSNotification");
        }

        public ActionResult StopSMSNotification()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StopSMSNotification(string shortCode, string serviceId, string criteria, string correlator)
        {
            var timestampString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string password = ConnectionDetails.HashPassword(ConnectionDetails.GetSpID() + ConnectionDetails.GetPassword() + timestampString);
            XNamespace soapenv = ConnectionDetails.SOAPRequestNamespaces["soapenv"];
            XNamespace v2 = ConnectionDetails.SOAPRequestNamespaces["v2"];
            //XNamespace loc = ConnectionDetails.SOAPRequestNamespaces["loc"];
            XElement soapEnvelope =
                new XElement(soapenv + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "soapenv", soapenv.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "v2", v2.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "loc", loc.NamespaceName),
                    new XElement(soapenv + "Header",
                            new XElement(v2 + "RequestSOAPHeader",
                                new XElement("spId", ConnectionDetails.GetSpID()),
                                new XElement("spPassword", password),
                                new XElement("serviceId", serviceId),
                                new XElement("timeStamp", timestampString)
                            )
                    ),
                    new XElement(soapenv + "Body",
                        new XElement(loc + "stopSmsNotification",
                            new XElement(loc + "correlator", correlator)
                        )
                    )
                );

            var notificationRequest = soapEnvelope.ToString();
            TempData["SmsNotificationResponse"] = Send(notificationRequest, timestampString);
            TempData["SmsNotificationRequest"] = notificationRequest;
            return View("StartSMSNotification");
        }

        private string Send(string message, string timestampString)
        {
            using (var handler = new HttpClientHandler() { Credentials = new NetworkCredential(ConnectionDetails.GetUsername(), ConnectionDetails.HashPassword(ConnectionDetails.GetSpID() + ConnectionDetails.GetPassword() + timestampString)) })
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("http://192.168.9.177:8310");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/SmsNotificationManagerService/services/SmsNotificationManager/");
                request.Content = new StringContent(message, Encoding.UTF8, "text/xml");
                string requestContentString = request.Content.ReadAsStringAsync().Result;
                var result = client.SendAsync(request).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                
                return resultContent;
            }
        }

    }
}