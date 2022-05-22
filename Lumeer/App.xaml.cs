﻿using Lumeer.Services;
using Lumeer.Views;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
[assembly: Dependency(typeof(NavigationService))]
[assembly: Dependency(typeof(SecureStorageService))]
namespace Lumeer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

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
