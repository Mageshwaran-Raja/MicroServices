using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new();
        // changes comes in the form of event --> event makes state changes

        public Guid Id 
        {
            get { return _id; }
        }
        public int Version { get; set; } = -1;

        // Method for Returning Uncommited Changes
        public IEnumerable<BaseEvent> GetUncommitedChanges() {
            return _changes;
        }

        // Method for Marking the Changes as commited
        public void MarkChangesAsCommited() {
            _changes.Clear(); // clears the list of events
        }

        // Method for Applying the Changes
        //events are used to make state changes to an aggregate
        private void ApplyChanges(BaseEvent @event, bool isNew) /* is this a new event*/
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });

            if (method == null) {
                throw new ArgumentNullException(nameof(method), @$"The Apply methos was not found in the aggregate for 
                            {@event.GetType().Name}!");
            }

            method.Invoke(this, new object[] {@event });

            if(isNew) {
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event) {
            ApplyChanges(@event, true);
        }
        
        // Method to Replay all the events to recreate the latest state of the aggregate
        // before new and uncommited changes have been applied
        public void ReplayEvents(IEnumerable<BaseEvent> events) // events retrieved from event store
        {
            foreach(var @event in events) {
                ApplyChanges(@event, false);
            }
        }
    }
}