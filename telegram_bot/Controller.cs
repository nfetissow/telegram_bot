using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using telegram_bot.translate;

namespace telegram_bot
{
    class Controller
    {
        //this probably needs to change when changing the token.
        public const long BOT_ID = 537933934;
        public const string BOT_NAME = "CSTest_my_bot";
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
                module = AddNewChat(message);
                if(module == null)
                {
                    return;//the add new chat method should care about printing a fitting error message
                }
            }
            Console.WriteLine(message.From.Username + ": " + message.Text);
            if("/StopDenBot".Equals(message.Text))
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
                        Debug.WriteLine("Exception when processing message. Message: " + message.ToString() + "\nexception: " + e);
                    }
                });
            }
        }

        TranslateModule AddNewChat(Message message)
        {
            //If we were just added to the chat we are in the message.NewChatMembers, however
            //if we were added to the chat before the bot was run we are not, so just add it and assume
            //we have been added to the chat at some point.
            //if(message.Chat.Type == ChatType.Channel) Maybe want to check at some point to avoid trying
            //to reply to channel where we can't write.
            TranslateModule module = new TranslateModule();
            modules.Add(message.Chat.Id, module);
            return module;
        }
    }
}
