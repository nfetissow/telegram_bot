using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace telegram_bot
{
    public interface MessageProcessedCallback
    {
        Task OnAnswerMessageGenerated(string message);
    }
}
