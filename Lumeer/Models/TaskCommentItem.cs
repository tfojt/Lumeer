using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.CommunityToolkit.UI.Views;

namespace Lumeer.Models
{
    public class TaskCommentItem
    {
        public TaskComment TaskComment { get; set; }

        public bool IsAuthor { get; private set; }

        public string CreationDateTxt { get; set; }

        public GravatarImageSource GravatarImageSource { get; set; }

        public TaskCommentItem(TaskComment taskComment)
        {
            TaskComment = taskComment;

            IsAuthor = taskComment.Author == Session.Instance.User.Id;

            CreationDateTxt = DateTimeOffset.FromUnixTimeMilliseconds(taskComment.CreationDate).ToString("MMMM d, yyyy, h:mm:ss tt");

            GravatarImageSource = new GravatarImageSource
            {
                Email = taskComment.AuthorEmail,
                Size = 15,
            };
        }

        public void UpdateData(TaskCommentItem other)
        {
            TaskComment.UpdateData(other.TaskComment);
            IsAuthor = other.IsAuthor;
        }
    }
}
