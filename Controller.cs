using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using telegram_bot.translate;

namespace telegram_bot
{
    class Controller
    {
        //this probably needs to change when changing the token.
        const long BOT_ID = 537933934;
        //id of bot is 537933934, when added to group gets new empty message with new user him. Also type of chat is Group

        readonly TelegramBotClient telegramClient = 
            new TelegramBotClient(/*change id when changing token*/"537933934:AAGJUyagFP4x2nhtZAD2OSqUZLPODfYFpEU");
        readonly Action terminateProgram;
        readonly Dictionary<long, TranslateModule> modules;

        public Controller(Action terminateProgram)
        {
            this.terminateProgram = terminateProgram;
            modules = new Dictionary<long, TranslateModule>();
            telegramClient.OnMessage += OnReceiveMessage;

            telegramClient.StartReceiving(TranslateModule.UpdateTypeFilterStatic.ToArray());
        }


        void OnReceiveMessage(object sender, MessageEventArgs args)
        {
            Message message = args.Message;
            long chatId = message.Chat.Id;
            TranslateModule module = modules.GetValueOrDefault(chatId, null);
            if(module == null)
            {
                AddNewChat(message);
            }
            Console.WriteLine(message.From.Username + ": " + message.Text);
            Console.WriteLine("Test: " + message);
            if(message.Text.Equals("/StopDenBot"))
            {
                terminateProgram();
                return;
            }

            if (module.MessageTypeFilter.Contains(message.Type))
            {
                Task.Run(async () =>  {
                    try
                    {
                        String answerMessage = await module.OnMessageReceived(message);
                        if (answerMessage != null)
                            await telegramClient.SendTextMessageAsync(chatId, answerMessage, replyToMessageId: message.MessageId);
                    } catch(Exception e)
                    {
                        Debug.WriteLine("Exception when processing message. Message: " + message + "\nexception: " + e);
                    }
                });
            }
        }

        void AddNewChat(Message message)
        {
            //This means we have not been in that chat before. 
            //check whether we have been added and then add new module for this chat.
            Boolean added = false;
            if (message.NewChatMembers != null)
            {
                foreach(User u in message.NewChatMembers)
                {
                    if(u.Id == BOT_ID)
                    {
                        modules.Add(message.Chat.Id, new TranslateModule());
                        added = true;
                    }
                }
            }

            if (!added)
            {
                //This is just in case there can be a message from an unkown chat without adding the bot.
                //Maybe it never will be called.
                Debug.WriteLine("Got message from unkown chat but not as a new member. message: " + message + " chatId: " + message.Chat.Id);
            }
        }
    }
}
