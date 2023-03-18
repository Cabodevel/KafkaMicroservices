using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
    public class EditCommentComand : BaseCommand
    {
        public Guid CommentId { get; set; }
        public string Comment { get; set; }
        public string UserName { get; set; }
    }
}
