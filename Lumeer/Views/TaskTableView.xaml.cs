using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskTableView : TableView
    {
        public TableSection TableSection { get; set; }

        public TaskTableView()
        {
            InitializeComponent();
            TableSection = tableSection;
        }
    }
}