using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public const long BOT_ID = 537933934;
        public const string BOT_NAME = "CSTest_my_bot";
        //id of bot is 537933934, when added to group gets new empty message with new user him. 
        //Also type of chat is Group

        readonly TelegramBotClient telegramClient = 
            new TelegramBotClient(/*change id when changing token*/APIKeys.TELEGRAM_KEY);
        readonly Action terminateProgram;
        readonly Dictionary<long, TranslateModule> modules= new Dictionary<long, TranslateModule>();

        public Controller(Action terminateProgram)
        {
            this.terminateProgram = terminateProgram;
            telegramClient.OnMessage += OnReceiveMessage;

            telegramClient.StartReceiving(TranslateModule.UpdateTypeFilterStatic.ToArray());
        }


        void OnReceiveMessage(object sender, MessageEventArgs args)
        {
            Message message = args.Message;
            Console.WriteLine(message.From.Username + ": " + message.Text);
            if ("/StopDenBot".Equals(message.Text))
            {
                terminateProgram();
                return;
            }
            if (!modules.ContainsKey(message.Chat.Id))
            {
                if (!AddNewChat(message))
                {
                    //the add new chat method should care about printing a fitting error message
                    return;
                }
            }
            PassMessageToModules(message);
        }

        void PassMessageToModules(Message message)
        {
            long chatId = message.Chat.Id;
            TranslateModule module = modules.GetValueOrDefault(chatId, null);
            if (module.MessageTypeFilter.Contains(message.Type))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await module.OnMessageReceived(message, 
                            new MessageProcessedCallbackImplementation(telegramClient, 
                            chatId, message.MessageId));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception when processing message. Message: " + message.ToString() + "\nexception: " + e);
                    }
                });
            }
        }

        bool AddNewChat(Message message)
        {
            //If we were just added to the chat we are in the message.NewChatMembers, however
            //if we were added to the chat before the bot was run we are not, so just add it and assume
            //we have been added to the chat at some point.
            //if(message.Chat.Type == ChatType.Channel) Maybe want to check at some point to avoid trying
            //to reply to channel where we can't write.
            TranslateModule module = new TranslateModule();
            modules.Add(message.Chat.Id, module);
            return module != null;
        }

        //TODO implement saving of modules

        class MessageProcessedCallbackImplementation : MessageProcessedCallback
        {
            private readonly long chatId;
            private readonly int messageId;
            private readonly TelegramBotClient telegramClient;

            public MessageProcessedCallbackImplementation(TelegramBotClient telegramClient, long chatId, int messageId)
            {
                this.chatId = chatId;
                this.messageId = messageId;
                this.telegramClient = telegramClient;
            }

            public async Task OnAnswerMessageGenerated(string message)
            {
                await telegramClient.SendTextMessageAsync(chatId,
                                    message, replyToMessageId: messageId);
            }
        }
    }
}
