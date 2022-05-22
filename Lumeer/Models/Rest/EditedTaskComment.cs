namespace Lumeer.Models.Rest
{
    public class EditedTaskComment
    {
        public string Id { get; set; }
        public string Comment { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public string ParentId { get; set; }

        public EditedTaskComment(TaskComment oldTaskComment, string editedComment)
        {
            Id = oldTaskComment.Id;
            Comment = editedComment;
            ResourceType = oldTaskComment.ResourceType;
            ResourceId = oldTaskComment.ResourceId;
            ParentId = oldTaskComment.ParentId;
        }
    }
}
