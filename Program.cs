using System;
using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Linq;
using QuickType;
using System.Reflection;

namespace ContactsScript
{
    class Program
    {
        static void Main(string[] args)
        {
            //OrgRequest test = new OrgRequest();
            //HttpWebResponse testresponse = test.MakeRequest();
            //string stringTest = ProcessRequest(testresponse);

            //JArray jArray = JArray.Parse();
            //JObject json = JObject.Parse(stringTest);


            try
            {
                string responseval = "";
                BasicRequest NewRequest = new BasicRequest();
                HttpWebResponse response = NewRequest.MakeRequest();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    List<string> errorLog = new List<string> 
                        {
                            "Bad Response",
                            response.StatusDescription
                        };
                        Logging.LogWrite(errorLog);
                }
                else
                {
                    responseval = ProcessRequest(response);
                    ContactCollection Contacts = new ContactCollection(responseval);
                    CSVCreator makeCSV = new CSVCreator();
                    //makeCSV.WriteCSV(Contacts);
                    makeCSV.WriteUploadCSV(Contacts);
                }

               
            }
            catch(Exception e)
            {
                List<string> errorLog = new List<string> 
                    {
                        "Error",
                        e.ToString()
                    };
                    Logging.LogWrite(errorLog);
            }
            
        }

        static public string ProcessRequest(HttpWebResponse response)
        {
            Stream receiveStream = response.GetResponseStream ();
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
            string responseVal = readStream.ReadToEnd();
            return responseVal;
        }
    }

    class OrgRequest 
    {
        private string testOrgId = "141099606";
        private string uri = "https://api.insightly.com/v3.1/Organisations/";
        private string APIAuth; 
        //141099606
        

        public HttpWebResponse MakeRequest()
        {
            //Reads Auth value from .txt file
            APIAuth = System.IO.File.ReadAllText(@"Auth.txt");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + testOrgId );
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(APIAuth));
            request.Headers.Add("Authorization", "Basic " + encoded);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
    
    class BasicRequest 
    {

        private string uri = "https://api.insightly.com/v3.1/Contacts?brief=false&count_total=false";
        private string APIAuth; 


        public HttpWebResponse MakeRequest()
        {
            //Reads Auth value from .txt file
            APIAuth = System.IO.File.ReadAllText(@"Auth.txt");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(APIAuth));
            request.Headers.Add("Authorization", "Basic " + encoded);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
      

        

    }

    class ContactCollection 
    {
        
        public List<InsightlyContact> Contacts { get; set; }


        public List<InsightlyContact> NewlyAdded { get ; set; }


        

        public void AddContact(InsightlyContact newContact)
        {
            Contacts.Add(newContact);
        }

        private void DateDelimited()
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);

            foreach(InsightlyContact contact in Contacts)
            {
                if(contact.DateCreatedUtc.DateTime >= yesterday)
                { 
                    NewlyAdded.Add(contact);
                }
            }

        }

        private void GenerateContactsFromResponseString(string response) 
        {
            JArray jArray = JArray.Parse(response);

            foreach(JObject contact in jArray)
            {
                string stringrep = Convert.ToString(contact);
                InsightlyContact insightlyContact = InsightlyContact.FromJson(stringrep);
                AddContact(insightlyContact);
            }


            DateDelimited();

        }

        public List<InsightlyContact> NewContacts()
        {

            return NewlyAdded;
        }

        public ContactCollection(string response)
        {
            
            Contacts = new List<InsightlyContact>();
            NewlyAdded = new List<InsightlyContact>();
            GenerateContactsFromResponseString(response);
        }
    }

    class CSVCreator
    {

        List<string> headers = new List<string>();

        //Dictionary<string,string> csvRowMap = new Dictionary<string,string>();

        public void WriteCSV(ContactCollection currentContacts)
        {
            
            var csv = new StringBuilder();
            
            List<string[]> csvrows = new List<string[]>();
            var contactForHeaders = currentContacts.Contacts[1];
            string[] headers = contactForHeaders.GetType().GetProperties().Select(x => x.Name).ToArray();
            csvrows.Add(headers);
            
            
            foreach(InsightlyContact record in currentContacts.NewContacts())
            {
                string[] row = record.GetType().GetProperties().Select(x => x.GetValue(record,null) != null ? x.GetValue(record,null).ToString(): "").ToArray();
                csvrows.Add(row);
            }
            
            File.WriteAllLines("Contacts.CSV", csvrows.Select(x => string.Join(",", x)));

        }

        public void WriteUploadCSV(ContactCollection currentContacts)
        {
            var csv = new StringBuilder();
            List<string[]> csvrows = new List<string[]>();

            //Simpler CSV Map. Outlook headers on the left, insightly props on the right
            Dictionary<string,string> csvRowMap = new Dictionary<string,string>()
            {
                {"BusinessAddressCity", "AddressMailCity"},
                {"BusinessAddressCountry", "AddressMailCountry"},
                {"BusinessAddressPostalCode", "AddressMailPostcode"},
                {"BusinessAddressState", "AddressMailState"},
                {"BusinessAddressStreet", "AddressMailStreet"},
                {"BusinessFaxNumber", "PhoneFax"},
                {"BusinessTelephoneNumber", "Phone"},
                {"Email1Address", "EmailAddress"},
                {"FirstName", "FirstName"},
                {"JobTitle", "Title"},
                {"LastName", "LastName"},
                {"MobileTelephoneNumber", "PhoneMobile"},
                {"Title", "Salutation"},
                {"BusinessAddress","BusinessAddressFull"},
                {"Categories","TagList"},
                {"CompanyName","OrgName"},
                {"FullName","FullName"}
            };

            string[] headers = csvRowMap.Keys.ToArray();
            string[] values = csvRowMap.Values.ToArray();
            csvrows.Add(headers);

            foreach(InsightlyContact record in currentContacts.NewContacts())
            {
                List<string> rowValList = new List<string>();
                foreach(string propname in values)
                {
                    string valstring = "";
                    if(record.GetType().GetProperty(propname).GetValue(record, null) != null)
                    {
                        valstring = record.GetType().GetProperty(propname).GetValue(record, null).ToString();
                    }
                    rowValList.Add(valstring);
                }

                csvrows.Add(rowValList.ToArray());
                //string[] row = record.GetType().GetProperties().Select(x => x.GetValue(record,null) != null ? x.GetValue(record,null).ToString(): "").ToArray();
                //csvrows.Add(row);
            }

            File.WriteAllLines("Contacts.CSV", csvrows.Select(x => string.Join(",", x)));

        }


    }

    static class Logging 
    {
        static private string loglocation = "contactsScriptLog.txt";

        static public void LogWrite(List<string> logwriteVals)
        {
            var log = new StringBuilder();
            string dateString = DateTime.Now.ToLongDateString();
            string logLine = dateString;
            foreach(string val in logwriteVals)
            {
                logLine += ", " + val;
            }
            log.AppendLine(logLine);
            File.AppendAllText(loglocation, log.ToString());
        }

    }

    

    

    
}
