using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot.translate {
    class TranslateModule
    {
        const string START_COMMAND = "startTranslating";
        const string STOP_COMMAND = "stopTranslating";
        const string SET_RULE_COMMAND = "setRule";

        Dictionary<string, Language> translationRules = new Dictionary<string, Language>();
        TranslationClient gTranslateClient = TranslationClient.Create();
        bool active = false;

        /// <summary>
        /// Invoked when the user sends a message that fits the filters of this module. Doesn't have to return immediately
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>An answer string to the user. If <c>null</c> is returned no answer will be sent.</returns>
        public async Task<string> OnMessageReceived(Message m)
        {
            if(m.Type == MessageType.Text)
            {
                string commando = Util.getCommandoString(m.Text);
                if (commando != null)
                {
                    switch (commando)
                    {
                        case START_COMMAND:
                            active = true;
                            return "Any text send in this group will be translated according ot the language rules.";
                        case STOP_COMMAND:
                            active = false;
                            return "No longer translating.";
                        case SET_RULE_COMMAND:
                            //add the language rule and 
                            return await AddTranslationRule(m.Text.Substring(SET_RULE_COMMAND.Length + 1));
                    }
                } else if(active)
                {
                    //message is a text to be translated:
                    return await TranslateText(m.Text);
                }
            }
            return null;
        }

        async Task<string> TranslateText(string message)
        {
            try
            {
                string languageCode = (await gTranslateClient.DetectLanguageAsync(message)).Language;
                //get the target language for translation if present
                Language targetLang = translationRules.GetValueOrDefault(languageCode, null);
                if (targetLang == null)
                    return null;

                return (await gTranslateClient.TranslateTextAsync(message, targetLang.Code, languageCode)).TranslatedText;
            } catch(Exception e) {
                //TODO look up is this the correct way or is there logging api?
                Debug.WriteLine("Exception when trying to translate. message:  " + message + " Exception: " + e);
                return null;
            }
            
            return null;
        }

        async Task<string> AddTranslationRule(string message)
        {
            string[] parts = message.Split(' ');
            //first check both languages are valid and supported
            IList<Language> languages = await gTranslateClient.ListLanguagesAsync();
            Language first = languages.First((l) => l.Code.Equals(parts[0]) || l.Name.Equals(parts[0]));
            if (first == null)
                return "Could not find language: " + parts[0];
            Language second = languages.First((l) => l.Code.Equals(parts[1]) || l.Name.Equals(parts[1]));
            if (second == null)
                return "Could not find language: " + parts[1];
            
            //now add them as rules for translation
            translationRules.Add(first.Code, second);
            return "Successfully added mapping from " + first.Name + " to " + second.Name;
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