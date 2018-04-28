using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace GonePostal
{
    class Program
    {

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        static void Main(string[] args)
        {
            var client = new RestClient("http://aramis.sancsoft.net:8123");
            TextReader tr = File.OpenText(@"c:/users/mterry/git/gonepostal/files/RWIZ-Quotes-2017-01-01-Present.csv");
            StreamWriter sw = new System.IO.StreamWriter(@"c:/users/mterry/git/gonepostal/files/RWIZ-Quotes-2017-01-01-Updated.csv",false);
            var csvr = new CsvReader(tr);
            csvr.Read();
            csvr.ReadHeader();
            var csvw = new CsvWriter(sw);
            while (csvr.Read())
            {
                var address = csvr["quoAddress"];
                var record = csvr.GetRecord<dynamic>();
                address = address.Replace("\r", "");
                address = address.Replace("\n", " ");
                Console.WriteLine(address);
                var request = new RestRequest("/parser", Method.POST);
                string requestData = "{\"query\":\"" + address + "\"}";
                request.AddParameter("application/json", requestData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                string content = response.Content;
                Console.WriteLine(content);
                JArray fields = JArray.Parse(content);
                PostalizedAddress pa = new PostalizedAddress(content);
                Console.WriteLine("Street Address = " + pa.StreetAddress());
                AddProperty(record, "quoStreetAddress", pa.StreetAddress());
                AddProperty(record, "quoCity", pa.City());
                AddProperty(record, "quoState", pa.State());
                AddProperty(record, "quoZip", pa.PostalCode());
                AddProperty(record, "quoCountry", pa.Country());
                //record.Add("quoStreetAddress", pa.StreetAddress());
                csvw.WriteRecord(record);
                csvw.NextRecord();
            }
            sw.Close();
        }
    }
}
