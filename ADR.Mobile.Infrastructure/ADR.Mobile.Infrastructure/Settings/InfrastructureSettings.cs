using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ADR.Mobile.Infrastructure.Settings
{
    /// <summary>
    /// Simple settings class to configure the connection to Windows Azure services.
    /// </summary>
    [XmlRoot("InfrastructureSettings", Namespace = XmlNamespace)]
    public class InfrastructureSettings
    {
        public const string XmlNamespace = @"urn:adayroi";

        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(InfrastructureSettings));
        private static readonly XmlReaderSettings readerSettings;

        static InfrastructureSettings()
        {
            //var schema = XmlSchema.Read(typeof(InfrastructureSettings).Assembly.GetManifestResourceStream("Infrastructure.Azure.Settings.xsd"), null);
            //readerSettings = new XmlReaderSettings() { ValidationType = ValidationType.Schema };
            //readerSettings.Schemas.Add(schema);
            readerSettings = new XmlReaderSettings();
        }

        /// <summary>
        /// Reads the settings from the specified file.
        /// </summary>
        public static InfrastructureSettings Read(string file)
        {
            //check file exist before Deserialize file 
            if (!System.IO.File.Exists(file)) throw new Exception("File not exist");
            using (var reader = XmlReader.Create(file, readerSettings))
            {
                return (InfrastructureSettings)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Reads the settings from the specified file.
        /// </summary>
        public static T Read<T>(string file)
        {
            //check file exist before Deserialize file 
            if (!System.IO.File.Exists(file)) throw new Exception("No file exist");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = XmlReader.Create(file, readerSettings))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    //[XmlRoot("InfrastructureSettings", Namespace = XmlNamespace)]
    //public class CustomXmlFormat : InfrastructureSettings
    //{
    //    //Anything goeas here
    //}
}
