using System.Threading.Tasks;

namespace Reusables.Cqrs
{
    /// <summary>
    /// Defines an interface for a command dispatcher. This dispatcher is responsible for distributing a command object to the right command handler, then letting it execute the command.
    /// </summary>
    public interface ICommandDispatcher
    {
        void Dispatch<TCommand>(TCommand command) where TCommand : Command;

        Task DispatchAsync<TAsyncCommand>(TAsyncCommand command) where TAsyncCommand : AsyncCommand;
    }
}
