using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.HomeViewModels
{
    public class FormItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ActionType { get; set; }
        public string ViewType { get; set; }
        public string FormName { get; set; }
    }
}
