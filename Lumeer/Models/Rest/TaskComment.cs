using Lumeer.Utils;
using System;

namespace Lumeer.Models.Rest
{
    public class TaskComment : NotifyPropertyChanged
    {
        public string _comment;
        public string Comment 
        {
            get => _comment;
            set => SetValue(ref _comment, value);
        }

        public object MetaData { get; set; }
        public string Id { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public string ParentId { get; set; }
        public long CreationDate { get; set; }
        public string Author { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorName { get; set; }

        public void UpdateData(TaskComment other)
        {
            if (Id != other.Id)
            {
                throw new ArgumentException("Ids do not match!");
            }

            Comment = other.Comment;
            MetaData = other.MetaData;
            ResourceType = other.ResourceType;
            ResourceId = other.ResourceId;
            ParentId = other.ParentId;
            CreationDate = other.CreationDate;
            Author = other.Author;
            AuthorEmail = other.AuthorEmail;
            AuthorName = other.AuthorName;
        }
    }
}
