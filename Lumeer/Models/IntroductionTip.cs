using Xamarin.Forms;

namespace Lumeer.Models
{
    public class IntroductionTip
    {
        public ImageSource ImageSource { get; set; }

        public string Tip { get; set; }

        public IntroductionTip(string imagePath, string tip)
        {
            ImageSource = ImageSource.FromResource(imagePath);
            Tip = tip;
        }
    }
}