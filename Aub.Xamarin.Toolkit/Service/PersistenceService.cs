using System.IO;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace Aub.Xamarin.Toolkit.Service
{
    public class PersistenceService : IPersistenceService
    {
        readonly Application _application;

        public PersistenceService(Application application)
        {
            _application = application;
        }

        public void Save(string key, string data)
        {
            _application.Properties[key] = data;
        }

        public void Save<T>(string key, T data)
        {
            _application.Properties[key] = Serlialize(data);
        }

        public string Get(string key)
        {
            object data;
            _application.Properties.TryGetValue(key, out data);
            return data.ToString();
        }

        public T Get<T>(string key)
        {
            object obj;
            if(_application.Properties.TryGetValue(key, out obj))
                return Deserliaze<T>(obj.ToString());
            return default(T);
        }

        public void Remove(string key)
        {
            _application.Properties.Remove(key);
        }

        string Serlialize<T>(T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, data);
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        T Deserliaze<T>(string value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(value))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
