using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;


namespace GonePostal
{
    public class PostalizedAddress
    {
        protected Dictionary<string, string> addressDict;

        public PostalizedAddress()
        {
            addressDict = new Dictionary<string, string>();
        }

        public PostalizedAddress(Dictionary<string, string> dict)
        {
            addressDict = dict;
        }

        public PostalizedAddress(string content)
        {
            JArray fields = JArray.Parse(content);
            addressDict = new Dictionary<string, string>();
            foreach (JObject obj in fields)
            {
                string key = "";
                string value = "";
                foreach (JProperty singleProp in obj.Properties())
                {
                    if (singleProp.Name.ToString() == "label")
                    {
                        key = singleProp.Value.ToString();
                    }
                    else if (singleProp.Name.ToString() == "value")
                    {
                        value = singleProp.Value.ToString();
                    }
                    string name = singleProp.Name;
                }
                if (!addressDict.ContainsKey(key))
                {
                    addressDict.Add(key, value);
                }
                else
                {
                    Console.WriteLine("Duplicate key = " + key + " with value = " + value);
                }
            }
        }

        protected string Append(string existing, string add)
        {
            string result = existing;
            if (add.Length > 0)
            {
                if (existing.Length > 0)
                {
                    result = existing + " " + add;
                }
                else
                {
                    result = add;
                }
            }
            return result;
        }

        protected string Value(string key, string def = "")
        {
            string value = def;
            if (addressDict.ContainsKey(key))
            {
                value = addressDict[key];
            }
            return value;
        }

        public string StreetAddress()
        {
            string streetAddress = "";
            streetAddress = Append(streetAddress, Value("house_number"));
            streetAddress = Append(streetAddress, Value("road"));
            return streetAddress;
        }

        public string City()
        {
            return Value("city");
        }
        public string State()
        {
            return Value("state");
        }
        public string PostalCode()
        {
            return Value("postcode");
        }

        public string Country()
        {
            return Value("country");
        }

    }
}
