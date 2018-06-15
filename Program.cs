using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace telegram_bot
{
    class Program
    {
        static bool ended = false;
        static TelegramBotClient _client = new TelegramBotClient("537933934:AAGJUyagFP4x2nhtZAD2OSqUZLPODfYFpEU");
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            _client.StartReceiving();
            _client.OnMessage += NewMessage;
            while(!ended)
            {
                continue;
            }
            _client.StopReceiving();
        }

        async static void NewMessage(object sender, MessageEventArgs Args)
        {
            if (Args.Message.Text == "stop")
                ended = !ended;
            Console.WriteLine(Args.Message.Chat.Username + ": " + Args.Message.Text);
            await _client.SendTextMessageAsync(Args.Message.Chat.Id, "You wrote " + Args.Message.Text);
        }
    }
}
