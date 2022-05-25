using Lumeer.Utils;

namespace Lumeer.Models
{
    public class LanguageItem : NotifyPropertyChanged
    {
        public string Name { get; set; }

        private bool _isSelected;
        public bool IsSelected 
        {
            get => _isSelected;
            set => SetValue(ref _isSelected, value);
        }

        public LanguageItem(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
