using Lumeer.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class LanguagesViewModel
    {
        public ObservableCollection<LanguageItem> Languages { get; set; } = new ObservableCollection<LanguageItem>();

        public ICommand SelectLanguageCmd => new Command<LanguageItem>(SelectLanguage);

        private LanguageItem _selectedLanguage;

        public LanguagesViewModel()
        {
            var englishLanguage = new LanguageItem("English");
            Languages.Add(englishLanguage);

            SelectLanguage(englishLanguage);
        }

        private void SelectLanguage(LanguageItem languageItem)
        {
            if (languageItem == _selectedLanguage)
            {
                return;
            }

            if (_selectedLanguage != null)
            {
                _selectedLanguage.IsSelected = false;
            }

            languageItem.IsSelected = true;
            _selectedLanguage = languageItem;
        }
    }
}
