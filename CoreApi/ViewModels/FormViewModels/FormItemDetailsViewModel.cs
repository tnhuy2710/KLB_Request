using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.FormViewModels
{
    public class FormItemDetailsViewModel
    {
        public long Id { get; set; }
        public string FormId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorTitle { get; set; }

        public DateTimeOffset SubmitDate { get; set; }

        public string HtmlContent { get; set; }

        public List<FormStepDetailsViewModel> StepsDetails { get; set; }
    }
}
