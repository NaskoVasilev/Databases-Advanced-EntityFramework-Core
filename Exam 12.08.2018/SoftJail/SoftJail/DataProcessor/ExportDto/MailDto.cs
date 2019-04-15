using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ExportDto
{
    [XmlType("Message")]
    public class MailDto
    {
        public string Description { get; set; }
    }
}
