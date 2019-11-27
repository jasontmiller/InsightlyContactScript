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
            //Request Object
            BasicRequest NewRequest = new BasicRequest();

            //Returns Get response as String
            string response = NewRequest.MakeRequest();

            //Creates a new collection of Contact Objects by parsing the response string
            ContactCollection Contacts = new ContactCollection(response);

            //New CSV Creator Object
            CSVCreator makeCSV = new CSVCreator();

            // Passses contacts collection to CSV for parsing/writing
            makeCSV.WriteCSV(Contacts);
        }
    }

    
    class BasicRequest 
    {

        private string uri = "https://api.insightly.com/v3.1/Contacts?brief=false&count_total=false";
        private string APIAuth = "ebe96c72-bdca-4e0a-bb65-430bf474f985";

        public string MakeRequest()
        {
            //ContactCollection contacts = new ContactCollection();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(APIAuth));
            request.Headers.Add("Authorization", "Basic " + encoded);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream ();
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
            string responseVal = readStream.ReadToEnd();
            return responseVal;
            

        }
      

        

    }

    class ContactCollection 
    {
        
        public List<InsightlyContact> Contacts { get; set; }

        //private List<InsightlyContact> NewlyAdded;

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

            // Creates list of new contacts.
            DateDelimited();

        }

        public List<InsightlyContact> NewContacts()
        {
            //DateDelimited();
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
        //private string csvName = "newCSV";
        List<string> headers = new List<string>();

        public void WriteCSV(ContactCollection currentContacts)
        {
            
            var csv = new StringBuilder();
            
            List<string[]> csvrows = new List<string[]>();
            var contactForHeaders = currentContacts.Contacts[1];
            string[] headers = contactForHeaders.GetType().GetProperties().Select(x => x.Name).ToArray();
            csvrows.Add(headers);
            
            //List<string> returnRows = new List<string>();
            foreach(InsightlyContact record in currentContacts.NewContacts())
            {
                string[] row = record.GetType().GetProperties().Select(x => x.GetValue(record,null) != null ? x.GetValue(record,null).ToString(): "").ToArray();
                csvrows.Add(row);
            }
            
            File.WriteAllLines("Contacts.CSV", csvrows.Select(x => string.Join(",", x)));

        }

    }


}
