using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class FontImageData : NotifyPropertyChanged
    {
        private string _fontFamily;
        public string FontFamily 
        {
            get => _fontFamily;
            set => SetValue(ref _fontFamily, value);
        }

        private string _glyph;
        public string Glyph
        {
            get => _glyph;
            set => SetValue(ref _glyph, value);
        }
        

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetValue(ref _color, value);
        }

        public FontImageData(string fontFamily, string glyph, Color color)
        {
            FontFamily = fontFamily;
            Glyph = glyph;
            Color = color;
        }

        public FontImageData(string fontFamily, string glyph, string colorHex) : this(fontFamily, glyph, Color.FromHex(colorHex))
        {
            
        }
    }
}
