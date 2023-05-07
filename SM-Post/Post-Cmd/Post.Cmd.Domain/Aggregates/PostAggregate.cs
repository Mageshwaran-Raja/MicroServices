using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new ();

        public bool Active
        {
            get => _active; set => _active = value;
        }

        public PostAggregate() { }
        
        // Create a new Post Event
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
            _author= @event.Author;
        }

        // Edit Message of the Post Event
        public void EditMessage(string message)
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot edit the message of inactive post");
            }

            if(string.IsNullOrWhiteSpace(message)) {
                throw new InvalidOperationException($"Value cannot be empty for {nameof(message)}");
            }

            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        // Like the Post Event
        public void LikePost()
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot live inactive post");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        // Add Comment for a Post event
        public void AddComment(string comment, string username)
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot add a comment for an inactive post");
            }

            if(string.IsNullOrWhiteSpace(comment)) {
                throw new InvalidOperationException($"Value cannot be empty for {nameof(comment)}");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                Username = username,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.Id, new Tuple<string, string>(@event.Comment, @event.Username));
        }

        // Edit Comment for a Post Event
        public void EditComment(Guid commentId, string comment, string username)
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot edit a comment of inactive post");
            }

            if(!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase)) {
                throw new InvalidOperationException($"You cannot edit the comment that was added by another author");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = username,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
        }

        // Remove a Comment for a Post Event
        public void RemoveComment(Guid commentId, string username)
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot remove a comment of inactive post");
            }

            if(!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase)) {
                throw new InvalidOperationException($"You cannot remove the comment that was added by another author");
            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        // Delete a Post Event
        public void DeletePost(string username)
        {
            if(!_active)
            {
                throw new InvalidOperationException("You cannot delete a post of inactive post");
            }

            if(!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase)) {
                throw new InvalidOperationException($"You cannot delete the post that posted added by another author");
            }

            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }

    }
}