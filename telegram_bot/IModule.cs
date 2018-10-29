using System.Collections.Immutable;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot
{
    delegate void IModuleChangedCallback();
    interface IModule
    {
        event IModuleChangedCallback OnModuleChanged;
        IImmutableSet<MessageType> MessageTypeFilter { get; }

        Task OnMessageReceived(Message message, IMessageProcessedCallback messageProcessedCallbackImplementation);
    }
}
