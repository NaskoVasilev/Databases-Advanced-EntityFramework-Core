using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace FastFood.DataProcessor.Dto.Import
{
    [XmlType("Item")]
    public class ImportItemOrderDto
    {
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
