// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var insightlyContact = InsightlyContact.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Net;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Newtonsoft.Json.Linq;

    public partial class InsightlyContact
    {
        [JsonProperty("CONTACT_ID")]
        public long ContactId { get; set; }

        [JsonProperty("SALUTATION")]
        public object Salutation { get; set; }

        [JsonProperty("FIRST_NAME")]
        public string FirstName { get; set; }

        [JsonProperty("LAST_NAME")]
        public string LastName { get; set; }

        [JsonProperty("IMAGE_URL")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("BACKGROUND")]
        public object Background { get; set; }

        [JsonProperty("VISIBLE_TO")]
        public string VisibleTo { get; set; }

        [JsonProperty("OWNER_USER_ID")]
        public long OwnerUserId { get; set; }

        [JsonProperty("VISIBLE_TEAM_ID")]
        public object VisibleTeamId { get; set; }

        [JsonProperty("DATE_CREATED_UTC")]
        public DateTimeOffset DateCreatedUtc { get; set; }

        [JsonProperty("DATE_UPDATED_UTC")]
        public DateTimeOffset DateUpdatedUtc { get; set; }

        [JsonProperty("SOCIAL_LINKEDIN")]
        public Uri SocialLinkedin { get; set; }

        [JsonProperty("SOCIAL_FACEBOOK")]
        public object SocialFacebook { get; set; }

        [JsonProperty("SOCIAL_TWITTER")]
        public object SocialTwitter { get; set; }

        [JsonProperty("DATE_OF_BIRTH")]
        public object DateOfBirth { get; set; }

        [JsonProperty("PHONE")]
        public object Phone { get; set; }

        [JsonProperty("PHONE_HOME")]
        public object PhoneHome { get; set; }

        [JsonProperty("PHONE_MOBILE")]
        public object PhoneMobile { get; set; }

        [JsonProperty("PHONE_OTHER")]
        public object PhoneOther { get; set; }

        [JsonProperty("PHONE_ASSISTANT")]
        public object PhoneAssistant { get; set; }

        [JsonProperty("PHONE_FAX")]
        public object PhoneFax { get; set; }

        [JsonProperty("EMAIL_ADDRESS")]
        public string EmailAddress { get; set; }

        [JsonProperty("ASSISTANT_NAME")]
        public object AssistantName { get; set; }

        [JsonProperty("ADDRESS_MAIL_STREET")]
        public object AddressMailStreet { get; set; }

        [JsonProperty("ADDRESS_MAIL_CITY")]
        public object AddressMailCity { get; set; }

        [JsonProperty("ADDRESS_MAIL_STATE")]
        public object AddressMailState { get; set; }

        [JsonProperty("ADDRESS_MAIL_POSTCODE")]
        public object AddressMailPostcode { get; set; }

        [JsonProperty("ADDRESS_MAIL_COUNTRY")]
        public object AddressMailCountry { get; set; }

        [JsonProperty("ADDRESS_OTHER_STREET")]
        public object AddressOtherStreet { get; set; }

        [JsonProperty("ADDRESS_OTHER_CITY")]
        public object AddressOtherCity { get; set; }

        [JsonProperty("ADDRESS_OTHER_STATE")]
        public object AddressOtherState { get; set; }

        [JsonProperty("ADDRESS_OTHER_POSTCODE")]
        public object AddressOtherPostcode { get; set; }

        [JsonProperty("ADDRESS_OTHER_COUNTRY")]
        public object AddressOtherCountry { get; set; }

        [JsonProperty("LAST_ACTIVITY_DATE_UTC")]
        public object LastActivityDateUtc { get; set; }

        [JsonProperty("NEXT_ACTIVITY_DATE_UTC")]
        public object NextActivityDateUtc { get; set; }

        [JsonProperty("CREATED_USER_ID")]
        public long CreatedUserId { get; set; }

        [JsonProperty("ORGANISATION_ID")]
        public object OrganisationId { get; set; }

        [JsonProperty("TITLE")]
        public object Title { get; set; }

        [JsonProperty("CUSTOMFIELDS")]
        public object[] Customfields { get; set; }

        [JsonProperty("TAGS")]
        public Tag[] Tags { get; set; }

        [JsonProperty("DATES")]
        public object[] Dates { get; set; }

    }

    public partial class Tag
    {
        [JsonProperty("TAG_NAME")]
        public string TagName { get; set; }
    }

    public partial class InsightlyContact
    {
        public static InsightlyContact FromJson(string json) => JsonConvert.DeserializeObject<InsightlyContact>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this InsightlyContact self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }


    //Adding functionality for conversion to import CSV

    public partial class InsightlyContact
    {
        //private string fullName;
        public string FullName
        {
            get { return FirstName + " " + LastName; }
            //set { fullName = value; }
        }

        //private string businessAddressFull;
        public string BusinessAddressFull
        {
            get { return 
            
            "\"" + AddressMailStreet +"\n" +
            AddressMailCity +"\n" +
            AddressMailState +"\n" +
            AddressMailPostcode +"\n" +
            AddressMailCountry+ "\"";}
            //set { businessAddressFull = value; }
        }

        //private string orgName;
        public string OrgName
        {
            get { return GetOrgName(); }
           //set { orgName = value; }
        }
        
        
        //private string tagList;
        public string TagList
        {
            get { 
                return CreateTagList();
                //return tagList; 
                
                }
            //set { tagList = value; }
        }

        private string CreateTagList()
        {
            string tagList = "";
            int tagCount = Tags.Count();
            foreach(Tag tagitem in Tags)
            {
                if (tagitem.TagName == Tags[tagCount -1].TagName)
                {
                    tagList += tagitem.TagName;
                }
                else
                {
                    tagList += tagitem.TagName + ", ";
                }
                
            }

            tagList = "\"" + tagList + "\"";

            return tagList;
        }

        private string GetOrgName()
        {
            
            OrgRequest getOrgName = new OrgRequest();
            string name = "";
            if(this.OrganisationId != null)
            {
                var orgName = getOrgName.MakeRequest(this.OrganisationId.ToString());
                string orgString = getOrgName.ProcessRequest(orgName);
                JObject json = JObject.Parse(orgString);
                name = json.SelectToken("ORGANISATION_NAME").ToString();
                
                
            }
            return name;
        }
        
        
    }

    class OrgRequest 
    {
        //private string testOrgId = "141099606";
        private string uri = "https://api.insightly.com/v3.1/Organisations/";
        private string APIAuth; 
        //141099606
        

        public HttpWebResponse MakeRequest(string orgID)
        {
            //Reads Auth value from .txt file
            APIAuth = System.IO.File.ReadAllText(@"Auth.txt");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri + orgID );
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(APIAuth));
            request.Headers.Add("Authorization", "Basic " + encoded);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public string ProcessRequest(HttpWebResponse response)
        {
            Stream receiveStream = response.GetResponseStream ();
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
            string responseVal = readStream.ReadToEnd();
            
            return responseVal;
        }
    }
}
