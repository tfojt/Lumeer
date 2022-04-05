using Lumeer.Services;
using Lumeer.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
[assembly: Dependency(typeof(NavigationService))]
namespace Lumeer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            /*Utils.ApiClient.Authorize();    // TODOT delete
            MainPage = new NavigationPage(new MainPage());*/
            MainPage = new IntroductionPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
