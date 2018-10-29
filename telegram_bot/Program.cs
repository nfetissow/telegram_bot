using System.Threading;
using Telegram.Bot;

namespace telegram_bot
{
    class Program
    {
        static TelegramBotClient _client = new TelegramBotClient(APIKeys.TELEGRAM_KEY);
        static Thread mainThread;
        
        static void Main(string[] args)
        {
            //this code runs the controller and waits until it stops the program.
            mainThread = Thread.CurrentThread;
            new Controller(() => mainThread.Interrupt());
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
