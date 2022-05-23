using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskActivityView : ContentView
    {
        public TaskActivityView(Models.Rest.Task task, Table table)
        {
            InitializeComponent();
            BindingContext = new TaskActivityViewModel(task, table);
        }
    }
}