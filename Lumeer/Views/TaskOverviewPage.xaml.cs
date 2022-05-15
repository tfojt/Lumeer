using Lumeer.Models.Rest;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskOverviewPage : ContentPage
    {
        public TaskOverviewPage(Models.Rest.Task task, Table table)
        {
            InitializeComponent();

            var detailTabViewItem = new TabViewItem()
            {
                Text = "Detail",
                Content = new TaskDetailView(task, table)
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
                Content = new TaskCommentsView()
            };
            this.tabView.TabItems.Add(commentsTabViewItem);

            var activityTabViewItem = new TabViewItem()
            {
                Text = "Activity",
                Content = new TaskActivityView()
            };
            this.tabView.TabItems.Add(activityTabViewItem);
        }
    }
}