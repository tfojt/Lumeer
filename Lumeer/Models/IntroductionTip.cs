using Xamarin.Forms;

namespace Lumeer.Models
{
    public class IntroductionTip
    {
        public ImageSource ImageSource { get; set; }

        public string Title { get; set; }
        public string Comment { get; set; }

        public IntroductionTip(string imageName, string title, string comment)
        {
            const string imagesLocation = "Lumeer.Images.";

            ImageSource = ImageSource.FromResource(imagesLocation + imageName);
            Title = title;
            Comment = comment;
        }
    }
}