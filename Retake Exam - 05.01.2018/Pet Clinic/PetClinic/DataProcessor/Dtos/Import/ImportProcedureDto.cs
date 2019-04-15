using System.Collections.Generic;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dtos.Import
{
    [XmlType("Procedure")]
    public class ImportProcedureDto
    {
        public string Vet { get; set; }

        public string Animal { get; set; }

        public string DateTime { get; set; }

        [XmlArray]
        public List<AnimalAidDto> AnimalAids { get; set; }
    }
}
