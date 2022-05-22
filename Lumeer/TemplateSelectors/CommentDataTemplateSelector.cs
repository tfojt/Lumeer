using Lumeer.Models;
using Xamarin.Forms;

namespace Lumeer.TemplateSelectors
{
    public class CommentDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentDataTemplate { get; set; }
        public DataTemplate CommentWithRightsDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var commentItem = (TaskCommentItem)item;

            return commentItem.IsAuthor ? CommentWithRightsDataTemplate : CommentDataTemplate;
        }
    }
}
