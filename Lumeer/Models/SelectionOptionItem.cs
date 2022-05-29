using Lumeer.Models.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class SelectionOptionItem
    {
        public string Name { get; set; }
        public Color BackgroundColor { get; set; }

        public SelectionOptionItem(SelectionOption selectionOption)
        {
            Name = selectionOption.EffectiveValue;
            BackgroundColor = Color.FromHex(selectionOption.Background);
        }
    }
}
