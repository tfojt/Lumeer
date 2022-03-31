using Lumeer.Services;
using Lumeer.ViewModels;
using Moq;
using NUnit.Framework;

namespace Lumeer.Tests
{
    public class MainViewModelTest
    {
        private readonly Mock<IAuthenticationService> authenticationServiceMock = new Mock<IAuthenticationService>();
        private readonly Mock<IAlertService> alertServiceMock = new Mock<IAlertService>();

        [Test]
        public void AuthenticationTest()
        {
            /*var viewModel = new MainViewModel(authenticationServiceMock.Object, alertServiceMock.Object);
            viewModel.AuthenticateCmd.Execute(null);*/
        }
    }
}
