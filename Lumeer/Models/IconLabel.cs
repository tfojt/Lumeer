using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models
{
    public class IconLabel
    {
        public FontImageData Icon { get; set; }
        public string Label { get; set; }

        public IconLabel(FontImageData icon, string label)
        {
            Icon = icon;
            Label = label;
        }
    }
}
