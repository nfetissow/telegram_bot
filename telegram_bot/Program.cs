using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace telegram_bot
{
    class Program
    {
        static bool ended = false;
        static TelegramBotClient _client = new TelegramBotClient(APIKeys.TELEGRAM_KEY);
        static Thread mainThread;
        
        static void Main(string[] args)
        {
            //this code runs the controller and waits until it stops the program.
            mainThread = Thread.CurrentThread;
            new Controller(() => mainThread.Interrupt());
            Thread.Sleep(Timeout.Infinite);
        }

        async static void NewMessage(object sender, MessageEventArgs Args)
        {

            if (Args.Message.Text == "stop")
                mainThread.Interrupt();
            Console.WriteLine(Args.Message.Chat.Username + ": " + Args.Message.Text);
            await _client.SendTextMessageAsync(Args.Message.Chat.Id, "You wrote " + Args.Message.Text);
        }
    }
}
