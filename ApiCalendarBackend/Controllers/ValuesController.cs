﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Text;
using System.Threading.Tasks;

namespace ApiCalendarBackend.Controllers
{
    public class TestController : ApiController
    {
        // Controller methods not shown...
    }
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly, GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Google Calendar amd Gmail API .NET Quickstart";

        string countdown(string fechaevento)
        {


         
            var hoy = new DateTime();
            hoy = DateTime.Now;

            double dias = 0;
            double horas = 0;
            double minutos = 0;
            double segundos = 0;
            
            var date1 = new DateTime();
            date1 = DateTime.ParseExact("2018-10-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
                                       System.Globalization.CultureInfo.InvariantCulture);

            double diferencia = (date1 - hoy).TotalDays / 1000;
            dias = Math.Floor(diferencia / 86400);
            diferencia = diferencia - (86400 * dias);
            horas = Math.Floor(diferencia / 3600);
            diferencia = diferencia - (3600 * horas);
            minutos = Math.Floor(diferencia / 60);
            diferencia = diferencia - (60 * minutos);
            segundos = Math.Floor(diferencia);



          //  console.log(hoy + "-" + fechaevento);
            var fechat = "";


            if (segundos > 0)
                fechat = "Quedan " + segundos + " segundos :";
            if (minutos > 0)
                fechat = "Quedan " + minutos + " minutos :";
            if (horas > 0)
            {
                if (date1.Hour > 12)
                    fechat = "Hoy a las " + date1.Hour + " PM :";
                else
                    fechat = "Hoy a las " + date1.Hour + " AM :";
            }
            if (dias > 0)
                fechat = "En " + dias + " Dias :";

            return fechat;


        }

        List<Labels> obtenerCorreos()
        {
            List<Labels> correos = new List<Labels>();
            UserCredential credential;

            using (var stream =
                new FileStream(HttpContext.Current.Server.MapPath("~/credentials2.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = HttpContext.Current.Server.MapPath("~/token2.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

            // List labels.
            IList<Label> labels = request.Execute().Labels;
            //Console.WriteLine("Labels:");
            if (labels != null && labels.Count > 0)
            {
                foreach (var labelItem in labels)
                {
                    Labels correo = new Labels();
                    correo.desc = labelItem.Name;
                    correos.Add(correo);
                    //correos.Add
                 //   Console.WriteLine("{0}", labelItem.Name);
                }
            }
            else
            {
                Labels correo = new Labels();
                correo.desc = "no hay correos";
                correos.Add(correo);
                //   Console.WriteLine("No labels found.");
            }
            return correos;
        }

        List<Event> megafunction()
        {
            CultureInfo ci = new CultureInfo(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy'-'MM'-'dd";

            ci.DateTimeFormat.LongTimePattern = "hh':'mm";

            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

            
            List<Event> megaCadena = new List<Event>();
            UserCredential credential;

            using (var stream =
                new FileStream(HttpContext.Current.Server.MapPath("~/credentials.json"), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = HttpContext.Current.Server.MapPath("~/token.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
       //         Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();


            //  Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    
                  
                    var dt = DateTime.Now;
                    string s = dt.ToString();

                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                         when = eventItem.Start.Date;
                      

                    }
                    //        Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    Event even = new Event();
                    even.date = when;
                    even.desc = eventItem.Summary;
                    megaCadena.Add(even);

                }


                //megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(events.Items);
                // megaCadena = megaCadena.Replace("\"", " ");
                // megaCadena = megaCadena.Substring(1, megaCadena.Length); ;
     
              
            }
            else
            {
                Event even = new Event();
                even.date = "";
                even.desc = "no hay eventos";
                megaCadena.Add(even);
           //     Console.WriteLine("No upcoming events found.");
            }
      //      Console.Read();
            return megaCadena;
        }

        public class Event
        {
            public String desc { get; set; }
            public String date { get; set; }
        }

        public class Labels
        {
            public String desc { get; set; }
        }
        public class RootObject
        {
            public List<Labels> labels { get; set; }
            public List<Event> events { get; set; }
        }
        // GET api/values
        public RootObject Get()
        {


              RootObject obj = new RootObject();
            //  List<Event> events = new List<Event>();
            // List<Labels> labels = new List<Labels>();


            obj.events = megafunction();
            obj.labels = obtenerCorreos();
         //      String megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
           //   RootObject objf = JsonConvert.DeserializeObject<RootObject>(megaCadena);
            
           //String megaCadena = Newtonsoft.Json.JsonConvert.SerializeObject(megafunction());
           
          
            return obj; 
        }

        // GET api/values/5
        public Event Get(int id)
        {
            

            return megafunction().ElementAt(id); 
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
