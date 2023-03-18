using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;

        private string _author;

        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

        public bool Active { get => _active; set { _active = value; } }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot edit a message of an inactive post");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message), "The value of message cannot be null or empty");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message,
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot like an inactive post");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = _id,
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot add a comment to an inactive post");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentNullException(nameof(comment), "The value of comment cannot be null or empty");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                UserName = userName,
                CommentDate = DateTime.Now,
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.UserName));
        }

        public void EditComment(Guid commentId, string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot edit a comment of an inactive post");
            }

            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user!");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                UserName = userName,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.UserName);
        }

        public void RemoveComment(Guid commentId, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("Cannot remove a comment of an inactive post");
            }

            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user!");
            }

            RaiseEvent(new CommentRemovedEvent { Id = _id, CommentId = commentId, });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("The post already has been removed");
            }

            if (!_author.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are noot allowed to delete a post that was made by someone else");
            }


            RaiseEvent(new PostRemovedEvent { Id = _id, });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
