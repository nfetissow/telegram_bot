using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot.translate {
    class TranslateModule {
        const string START_COMMAND = "startTranslating";
        const string SET_RULE_COMMAND = "setRule";

        /// <summary>
        /// Invoked when the user sends a message that fits the filters of this module.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task<string> OnMessageReceived(Message m)
        {
            if(m.Type == MessageType.Text)
            {
                string commando = Util.getCommandoString(m.Text);
                switch(commando)
                {
                    case START_COMMAND:

                        break;
                    case SET_RULE_COMMAND:

                        break;
                }
            }
            return null;
        }

        public IImmutableSet<UpdateType> UpdateTypeFilter { get; } = 
            new HashSet<UpdateType>()
            {
                UpdateType.Message
            }.ToImmutableHashSet();

        public IImmutableSet<MessageType> MessageTypeFilter { get; } =
            new HashSet<MessageType>()
            {
                MessageType.Text
            }.ToImmutableHashSet();
    }
}