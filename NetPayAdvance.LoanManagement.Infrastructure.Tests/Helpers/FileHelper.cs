using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace NetPayAdvance.Transactions.Tests.Helpers
{
    public static class FileHelper
    {
        public static XmlDocument LoadXml(string name)
        {
            string xmlFile = LoadString($@"{name}.xml");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlFile);
            return xmlDocument;
        }

        public static T LoadJson<T>(string name)
        {
            string jsonFile = LoadString($@"{name}.json");
            return JsonConvert.DeserializeObject<T>(jsonFile);
        }

        public static string LoadString(string name) => File.ReadAllText($@"assets/{name}", Encoding.UTF8);
    }
}