using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.MarkupExtensions
{
    [ContentProperty(nameof(Resource))]
    public class EmbeddedImage : IMarkupExtension
    {
        public string Resource { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrWhiteSpace(Resource))
            {
                return null;
            }

            return ImageSource.FromResource(Resource);
        }
    }
}
