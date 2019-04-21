using System.Xml.Serialization;

namespace MusicHub.DataProcessor.ImportDtos
{
    [XmlType("Song")]
    public class PerformerSongsImportDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
