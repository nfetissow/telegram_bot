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
        const string ADD_RULE_COMMAND = "addRule";
        const string HELP_COMMAND = "help";

        Dictionary<string, Language> translationRules = new Dictionary<string, Language>();
        TranslationClient gTranslateClient = TranslationClient.Create();
        bool active = false;

        /// <summary>
        /// Invoked when the user sends a message that fits the filters of this module. Doesn't have to return immediately
        /// </summary>
        /// <param name="m">The message.</param>
        /// <returns>An answer string to the user. It will be sent as a reply to the message.
        /// If <c>null</c> is returned no answer will be sent.</returns>
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
                        case ADD_RULE_COMMAND:
                            //the parameters for the new rule
                            string parameters = m.Text.Substring(ADD_RULE_COMMAND.Length + 1);
                            //add the language rule. 
                            return await AddTranslationRule(parameters);
                        case HELP_COMMAND:
                            return "Possible commands for the translate bots are:\n" +
                                " -/" + START_COMMAND + " to start translating every message that fits the translating rules.\n" +
                                " -/" + STOP_COMMAND + " to stop translating every message.\n" +
                                " -/" + ADD_RULE_COMMAND + " [from lang] [to lang] to add a new translation rule.";
                    }
                } else if(active)
                {
                    //message is a text to be translated:
                    return await TranslateText(m.Text);
                }
            }
            return null;
        }

        /// <summary>
        /// Detect the language of the message
        /// </summary>
        /// <param name="message">The text to be translated.</param>
        /// <returns>The translated text or <c>null</c> if either the translation 
        /// failed or the text was empty or there was no translation rule for this language.</returns>
        async Task<string> TranslateText(string message)
        {
            if (String.IsNullOrWhiteSpace(message))
                return null;
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

        //TODO the message type filter is used when receving a message, the update type filter
        //can be directly set when asking for updates. That means the latter needs to be static as well.
        //It for sure is not good code style to just duplicate the variable. How to do?

        public IImmutableSet<MessageType> MessageTypeFilter { get; } =
            new HashSet<MessageType>()
            {
                MessageType.Text
            }.ToImmutableHashSet();

        public IImmutableSet<UpdateType> UpdateTypeFilter { get; } = UpdateTypeFilterStatic;

        public static IImmutableSet<UpdateType> UpdateTypeFilterStatic { get; } =
            new HashSet<UpdateType>()
            {
                UpdateType.Message
            }.ToImmutableHashSet();
    }
}