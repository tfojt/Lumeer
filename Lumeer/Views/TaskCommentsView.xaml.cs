using Lumeer.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskCommentsView : ContentView
    {
        public TaskCommentsView(Models.Rest.Task task)
        {
            InitializeComponent();

            var viewModel = new TaskCommentsViewModel(task);
            viewModel.ScrollRequested += ViewModel_ScrollRequested; ;

            BindingContext = viewModel;
        }

        private void ViewModel_ScrollRequested(Models.TaskCommentItem comment, ScrollToPosition position, bool animated)
        {
            listView.ScrollTo(comment, position, animated);
        }
    }
}