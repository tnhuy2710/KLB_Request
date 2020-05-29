using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.FormViewModels
{
    public class FormSubmitViewModel
    {
        public string EmpCodeNextStep { get; set; }
        public List<CellSubmit> Cells { get; set; }
    }

    public class CellSubmit
    {
        public string Address { get; set; }
        public string Value { get; set; }
        public string Base { get; set; }
    }
}
