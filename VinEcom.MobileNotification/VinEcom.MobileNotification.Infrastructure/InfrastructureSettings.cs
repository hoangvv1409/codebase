using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace VinEcom.MobileNotification.Infrastructure
{
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
            if (!System.IO.File.Exists(file))
                return null;
            using (var reader = XmlReader.Create(file, readerSettings))
            {
                return (InfrastructureSettings)serializer.Deserialize(reader);
            }
        }

        public ServiceBusSettings ServiceBus { get; set; }
    }
}
