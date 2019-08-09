using System.Threading.Tasks;

namespace telegram_bot
{
    public interface IMessageProcessedCallback
    {
        Task OnAnswerMessageGenerated(string message);
    }
}
