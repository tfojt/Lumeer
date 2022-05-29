using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewLinkTypePopup : Popup<NewLinkType>
    {
        public NewLinkTypePopup(Table currentTable)
        {
            InitializeComponent();
            var newLinkTypeViewModel = new NewLinkTypeViewModel(currentTable);
            newLinkTypeViewModel.CreatingDone += NewLinkTypeViewModel_CreatingDone;
            BindingContext = newLinkTypeViewModel;
        }

        private void NewLinkTypeViewModel_CreatingDone(NewLinkType newLinkType)
        {
            Dismiss(newLinkType);
        }
    }
}