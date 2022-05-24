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
        public TaskDetailView TaskDetailView { get; set; }

        public TaskOverviewPage(Models.Rest.Task task, Table table)
        {
            InitializeComponent();

            TaskOverviewViewModel = new TaskOverviewViewModel(task);
            BindingContext = TaskOverviewViewModel;

            TaskDetailView = new TaskDetailView(task, table);
            var detailTabViewItem = new TabViewItem()
            {
                Text = "Detail",
                Content = TaskDetailView
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
            await TaskDetailView.TaskDetailViewModel.SaveCmd.ExecuteAsync();
        }
    }
}