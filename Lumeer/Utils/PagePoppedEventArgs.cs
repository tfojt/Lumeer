using System;
using Xamarin.Forms;

namespace Lumeer.Utils
{
    public class PagePoppedEventArgs : EventArgs
    {
        public Page Page { get; set; }

        public PagePoppedEventArgs(Page page)
        {
            Page = page;
        }
    }
}
