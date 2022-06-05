using Lumeer.Fonts;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class FontImageData : NotifyPropertyChanged
    {
        private const string DEFAULT_FONT_FAMILY = FontAwesomeAliases.PRO_REGULAR;
        private const string DEFAULT_GLYPH = FontAwesomeIcons.CircleQuestion;
        private static readonly Color DEFAULT_COLOR = Color.Black;

        public static FontImageData Default => new FontImageData();

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

        private static Dictionary<string, string> _glyphCache = new Dictionary<string, string>();

        private FontImageData() : this(DEFAULT_FONT_FAMILY, DEFAULT_GLYPH, DEFAULT_COLOR)
        {
        }

        public FontImageData(string fontFamily, string glyph, Color color)
        {
            Init(fontFamily, glyph, color);
        }

        public FontImageData(string iconRestFormat, string colorHex)
        {
            string fontFamily = null;
            string glyph = null;

            try
            {
                string[] iconPartsRestFormat = iconRestFormat.Split(' ');   // fas fa-computer-classic
                string fontFamilyRestFormat = iconPartsRestFormat[0];
                fontFamily = ParseFontFamily(fontFamilyRestFormat);

                string iconWithPrefixRestFormat = iconPartsRestFormat[1];
                glyph = ParseGlyph(iconWithPrefixRestFormat);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing {iconRestFormat}: {ex}");

                if (string.IsNullOrEmpty(fontFamily))
                {
                    fontFamily = DEFAULT_FONT_FAMILY;
                }

                if (string.IsNullOrEmpty(glyph))
                {
                    glyph = DEFAULT_GLYPH;
                }
            }

            Init(fontFamily, glyph, colorHex);
        }

        private void Init(string fontFamily, string glyph, string colorHex)
        {
            Init(fontFamily, glyph, Color.FromHex(colorHex));
        }
        
        private void Init(string fontFamily, string glyph, Color color)
        {
            FontFamily = fontFamily;
            Glyph = glyph;
            Color = color;
        }

        private string ParseFontFamily(string fontFamilyRestFormat)
        {
            string fontFamily;

            if (fontFamilyRestFormat == "fas")
            {
                fontFamily = FontAwesomeAliases.PRO_SOLID;
            }
            else if (fontFamilyRestFormat == "far")
            {
                fontFamily = FontAwesomeAliases.PRO_REGULAR;
            }
            else if (fontFamilyRestFormat == "fal")
            {
                fontFamily = FontAwesomeAliases.PRO_LIGHT;
            }
            else if (fontFamilyRestFormat == "fat")
            {
                fontFamily = FontAwesomeAliases.PRO_THIN;
            }
            else
            {
                fontFamily = DEFAULT_FONT_FAMILY;
                Debug.WriteLine($"Could not parse font family {fontFamily}");
            }

            return fontFamily;
        }

        private string ParseGlyph(string iconWithPrefixRestFormat)
        {
            string[] iconWithPrefixPartsRestFormat = iconWithPrefixRestFormat.Split('-');
            string iconName = "";

            for (int i = 1; i < iconWithPrefixPartsRestFormat.Length; i++)
            {
                string iconNamePart = iconWithPrefixPartsRestFormat[i];
                iconName += iconNamePart;
            }

            if (!_glyphCache.TryGetValue(iconName, out string glyph))
            {
                FieldInfo glyphField = typeof(FontAwesomeIcons)
                    .GetFields()
                    .FirstOrDefault(f => string.Equals(f.Name, iconName, StringComparison.OrdinalIgnoreCase));

                if (glyphField != null)
                {
                    glyph = (string)glyphField.GetValue(null);
                }
                else
                {
                    glyph = DEFAULT_GLYPH;
                    Debug.WriteLine($"Could not parse glyph {iconName}");
                }

                _glyphCache.Add(iconName, glyph);
            }

            return glyph;
        }
    }
}
