using Lumeer.Utils;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.Models.Rest
{
    public class Task : NotifyPropertyChanged
    {
        public Dictionary<string, object> Data { get; set; }
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
            set => SetValue(ref _favorite, value);
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
