using Lumeer.Models.Rest;
using Lumeer.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskOverviewPage : ContentPage
    {
        public TaskOverviewViewModel TaskOverviewViewModel { get; set; }
        private TaskDetailView _taskDetailView;

        public TaskOverviewPage(Models.Rest.Task task, Table table)
        {
            InitializeComponent();

            TaskOverviewViewModel = new TaskOverviewViewModel(task);
            BindingContext = TaskOverviewViewModel;

            _taskDetailView = new TaskDetailView(task, table);
            var detailTabViewItem = new TabViewItem()
            {
                Text = "Detail",
                Content = _taskDetailView
            };
            this.tabView.TabItems.Add(detailTabViewItem);

            var linksTabViewItem = new TabViewItem()
            {
                Text = "Links",
                Content = new TaskLinksView()
            };
            this.tabView.TabItems.Add(linksTabViewItem);

            var commentsTabViewItem = new TabViewItem()
            {
                Text = "Comments",
                Content = new TaskCommentsView(task)
            };
            this.tabView.TabItems.Add(commentsTabViewItem);

            var activityTabViewItem = new TabViewItem()
            {
                Text = "Activity",
                Content = new TaskActivityView(task, table)
            };
            this.tabView.TabItems.Add(activityTabViewItem);
        }

        protected async override void OnDisappearing()
        {
            await _taskDetailView.TaskDetailViewModel.SaveCmd.ExecuteAsync();
        }
    }
}