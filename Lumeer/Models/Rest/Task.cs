using Lumeer.Utils;
using System;
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
        public long UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
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

        public void UpdateData(Task other)
        {
            if (Id != other.Id)
            {
                throw new ArgumentException("Ids do not match!");
            }

            //Data = other.Data;
            /*foreach (KeyValuePair<string, object> kvp in other.Data)
            {
                if (Data.ContainsKey(kvp.Key))
                {
                    Data[kvp.Key] = kvp.Value;
                }
                else
                {
                    Data.Add(kvp.Key, kvp.Value);
                }
            }*/

            CollectionId = other.CollectionId;
            CreationDate = other.CreationDate;
            UpdatedDate = other.UpdatedDate;
            CreatedBy = other.CreatedBy;
            UpdatedBy = other.UpdatedBy;
            DataVersion = other.DataVersion;
            MetaData = other.MetaData;
            CommentsCount = other.CommentsCount;
            Favorite = other.Favorite;
        }
    }
}
