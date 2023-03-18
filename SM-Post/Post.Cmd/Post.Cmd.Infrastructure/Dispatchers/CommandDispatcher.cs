﻿using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers
{
    internal class CommandDispatcher : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            if (_handlers.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException("Cannot register twice");
            }

            _handlers.Add(typeof(T), x => handler((T)x));
        }

        public async Task SendAsync(BaseCommand command)
        {
            if(_handlers.TryGetValue(command.GetType(), out var handler)) 
            {
                await handler(command);
            }
            else
            {
                throw new InvalidOperationException("No command handler registered");
            }
        }
    }
}
