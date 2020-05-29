using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.FormViewModels
{
    public class FormConfirmViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset CloseDate { get; set; }

        public IList<FormConfirmItemViewModel> Forms { get; set; }

        public FormConfirmViewModel()
        {
            Forms = new List<FormConfirmItemViewModel>();
        }
    }
}
