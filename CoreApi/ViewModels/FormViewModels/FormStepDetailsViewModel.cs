using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.FormViewModels
{
    public class FormStepDetailsViewModel
    {
        public string StepId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InfoTitle { get; set; }
        public string InfoDescription { get; set; }

        public string Claims { get; set; }

        public int Index { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
    }
}
