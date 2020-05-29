using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.ViewModels.HomeViewModels
{
    public class HomeViewModel
    {
        public IList<FormItemViewModel> EditForms { get; set; }
        public IList<FormItemViewModel> ConfirmForms { get; set; }
        public IList<FormItemViewModel> ViewForms { get; set; }
        //public IList<FormItemViewModel> HomeForms { get; set; }

        public HomeViewModel()
        {
            EditForms = new List<FormItemViewModel>();   
            ConfirmForms = new List<FormItemViewModel>();   
            ViewForms = new List<FormItemViewModel>();

          //  HomeForms = new List<FormItemViewModel>();
        }
    }
}
