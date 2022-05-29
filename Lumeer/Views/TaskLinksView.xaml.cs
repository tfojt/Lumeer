using Lumeer.Models.Rest;
using Lumeer.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskLinksView : ContentView
    {
        public TaskLinksView(Models.Rest.Task task, Table table)
        {
            InitializeComponent();
            BindingContext = new TaskLinksViewModel(task, table);
        }
    }
}