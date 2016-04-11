using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ADR.Mobile.Infrastructure.Settings;

namespace ADR.Mobile.Infrastructure.Messaging.ServiceBus
{
    [XmlRoot("ServiceBus", Namespace = InfrastructureSettings.XmlNamespace)]
    public class ServiceBusSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusSettings"/> class.
        /// </summary>
        public ServiceBusSettings()
        {
            this.Topics = new List<TopicSettings>();
        }

        public string ConnectionString { get; set; }

        [XmlArray(ElementName = "Topics", Namespace = InfrastructureSettings.XmlNamespace)]
        [XmlArrayItem(ElementName = "Topic", Namespace = InfrastructureSettings.XmlNamespace)]
        public List<TopicSettings> Topics { get; set; }
    }

    [XmlRoot("Topic", Namespace = InfrastructureSettings.XmlNamespace)]
    public class TopicSettings
    {
        public TopicSettings()
        {
            this.Subscriptions = new List<SubscriptionSettings>();
            this.MigrationSupport = new List<UpdateSubscriptionIfExists>();
        }

        [XmlAttribute]
        public bool IsEventBus { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlIgnore]
        public TimeSpan DuplicateDetectionHistoryTimeWindow { get; set; }

        [XmlElement("Subscription", Namespace = InfrastructureSettings.XmlNamespace)]
        public List<SubscriptionSettings> Subscriptions { get; set; }

        [XmlArray(ElementName = "MigrationSupport", Namespace = InfrastructureSettings.XmlNamespace)]
        [XmlArrayItem(ElementName = "UpdateSubscriptionIfExists", Namespace = InfrastructureSettings.XmlNamespace)]
        public List<UpdateSubscriptionIfExists> MigrationSupport { get; set; }

        /// <summary>
        /// Don't access this property directly. Use the properly typed 
        /// <see cref="DuplicateDetectionHistoryTimeWindow"/> instead.
        /// </summary>
        /// <remarks>
        /// XmlSerializer still doesn't know how to convert TimeSpan... 
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlAttribute("DuplicateDetectionHistoryTimeWindow")]
        public string XmlDuplicateDetectionHistoryTimeWindow
        {
            get { return this.DuplicateDetectionHistoryTimeWindow.ToString("hh:mm:ss"); }
            set { this.DuplicateDetectionHistoryTimeWindow = TimeSpan.Parse(value); }
        }
    }

    [XmlRoot("Subscription", Namespace = InfrastructureSettings.XmlNamespace)]
    public class SubscriptionSettings
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool RequiresSession { get; set; }

        [XmlAttribute]
        public string SqlFilter { get; set; }
    }

    [XmlRoot("UpdateSubscriptionIfExists", Namespace = InfrastructureSettings.XmlNamespace)]
    public class UpdateSubscriptionIfExists
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string SqlFilter { get; set; }
    }
}
