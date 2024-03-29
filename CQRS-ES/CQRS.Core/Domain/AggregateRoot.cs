using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid _id;
        private readonly List<BaseEvent> _changes = new();

        public Guid Id { get => _id; }

        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUncommitedChanges() => _changes;

        public void MarkChangeAsCommited()
        {
            _changes.Clear();
        }

        private void ApplyChange(BaseEvent @event, bool isNew)
        {
            var type = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).First(x => x.Name == @event.Type);
            var method = GetType().GetMethod("Apply", new Type[] { type });

            if(method is null)
            {
                throw new InvalidOperationException($"The Apply method was not found in the aggregate for {type.Name}");
            }

            method.Invoke(this, new object[] { @event, });

            if(isNew )
            {
                _changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event) 
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach(BaseEvent @event in events)
            {
                ApplyChange(@event, false);
            }
        }
    }
}
