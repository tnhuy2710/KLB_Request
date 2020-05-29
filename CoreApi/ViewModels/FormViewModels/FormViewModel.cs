using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Models;

namespace CoreApi.ViewModels.FormViewModels
{
    public class FormViewModel
    {
        public string FormId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int CurrentStepIndex { get; set; }
        public IList<FormStep> Steps { get; set; }
        public List<FormStepDetailsViewModel> StepsDetails { get; set; }

        public string FormHtmlContent { get; set; }

        public bool IsAllowEditable { get; set; }
    }
}
