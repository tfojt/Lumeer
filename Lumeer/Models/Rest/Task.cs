using Lumeer.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.Models.Rest
{
    public class Task : NotifyPropertyChanged
    {
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public ObservableCollection<ObservableKeyValuePair<string, object>> ObservableData { get; set; }
        public string Id { get; set; }
        public string CollectionId { get; set; }
        public long CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public int DataVersion { get; set; }
        public object MetaData{ get; set; }
        public int CommentsCount{ get; set; }

        private bool _favorite;
        public bool Favorite
        {
            get => _favorite;
            set
            {
                SetValue(ref _favorite, value);
                FavoriteText = _favorite ? "Favorite" : "Not favorite";
            }
        }

        private string _favoriteText;
        public string FavoriteText
        {
            get => _favoriteText;
            set => SetValue(ref _favoriteText, value);
        }

        public ICommand FavoriteCmd { get; set; }

        public Task()
        {
            FavoriteCmd = new Command(ChangeFavoriteStatus);
        }

        private void ChangeFavoriteStatus()
        {
            Favorite = !Favorite;
        }
    }
}
