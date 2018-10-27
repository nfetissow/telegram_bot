using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace telegram_bot
{
    class Program
    {
        static bool ended = false;
        static TelegramBotClient _client = new TelegramBotClient("537933934:AAGJUyagFP4x2nhtZAD2OSqUZLPODfYFpEU");
        static Thread mainThread;
        
        static void Main(string[] args)
        {
            //this code runs the controller and waits until it stops the program.
            mainThread = Thread.CurrentThread;
            new Controller(() => mainThread.Interrupt());
            Thread.Sleep(Timeout.Infinite);

            //the following code is for testing purposes
            /*Console.WriteLine("Hello World!");
            mainThread = Thread.CurrentThread;
            _client.StartReceiving();
            _client.OnMessage += NewMessage;
            //Thread will sleep until interrupted by Thread.Interrupt();
            //better than an infinite loop.
            Thread.Sleep(Timeout.Infinite);
            //_client.StopReceiving();*/
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
