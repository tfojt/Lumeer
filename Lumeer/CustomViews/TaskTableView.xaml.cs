using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.CustomViews
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