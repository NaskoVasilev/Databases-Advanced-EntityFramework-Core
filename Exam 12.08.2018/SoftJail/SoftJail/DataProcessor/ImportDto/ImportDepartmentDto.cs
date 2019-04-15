using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentDto
    {
        [MinLength(3), MaxLength(25)]
        public string Name { get; set; }

        public ICollection<ImportCellDto> Cells { get; set; }
    }
}
