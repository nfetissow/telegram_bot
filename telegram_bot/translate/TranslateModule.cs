using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot.translate
{
    [Serializable]
    class TranslateModule: IModule, IDeserializationCallback
    {
        const string START_COMMAND = "startTranslating";
        const string STOP_COMMAND = "stopTranslating";
        const string ADD_RULE_COMMAND = "addRule";
        const string HELP_COMMAND = "help";
        const string LANGS_COMMAND = "languages";
        private const string START_MESSAGE = "Any text send in this group will be " +
            "translated according to the language rules.";
        private const string STOP_MESSAGE = "No longer translating.";
        private const string HELP_MESSAGE = "Possible commands for the translate bots are:\n" +
            " -/" + START_COMMAND + " to start translating every message that fits the " +
            "translating rules.\n" +
            " -/" + STOP_COMMAND + " to stop translating every message.\n" +
            " -/" + ADD_RULE_COMMAND + " [from lang] [to lang] to add a new translation rule.\n" +
            " -/" + LANGS_COMMAND + " to get a list of all supported languages.";
        private const string FIRST_MESSAGE = "Type /help to see commands for this bot.";
        private const string RULE_INVALID_MESSAGE = "I am sorry, this specific translation rule " +
            "is not available. Please use the /" + LANGS_COMMAND + " to get a list of all supported" +
            "languages. Not all combinations of two languages may be supported.";
        private const string INSUFFICIENT_RULE_PARAMETERS_MESSAGE =
            "A language rule needs exactly two arguments: " +
            "The source language and the target language.";
        private const string INVALID_LANG_CODE_MESSAGE = " is not a valid language code. " +
            "Please use /" + LANGS_COMMAND + " to get a list of all supported languages";
        private const string TRANSLATION_FAILED_MESSAGE = 
            "I are sorry, an unexpected error occured. I could not translate the message.";

        [field: NonSerialized]
        public event IModuleChangedCallback OnModuleChanged;

        readonly Dictionary<string, string> translationRules = new Dictionary<string, string>();
        bool active = false;
        bool firstMessage = true;

        [NonSerialized]
        YandexTranslateService yandexTranslationService;

        public TranslateModule()
        {
            //because of deserialization I need a common way to do this.
            OnDeserialization(null);
        }

        public async Task OnMessageReceived(Message m, IMessageProcessedCallback callback)
        {
            if(m.Type == MessageType.Text && !string.IsNullOrWhiteSpace(m.Text))
            {
                Command command = Command.TryParse(m.Text);
                if (command != null)
                {
                    await callback.OnAnswerMessageGenerated(await ReactToCommand(command));
                }
                else if (active)
                {
                    //message is a text to be translated:
                    await TranslateText(m.Text.Trim(), callback);
                }
                else if (firstMessage)
                {
                    await callback.OnAnswerMessageGenerated(FIRST_MESSAGE);
                }
                firstMessage = false;
            }
        }

        async Task<string> ReactToCommand(Command command)
        {
            switch (command.CommandCode)
            {
                case START_COMMAND:
                    active = true;
                    OnModuleChanged();
                    return START_MESSAGE;
                case STOP_COMMAND:
                    active = false;
                    OnModuleChanged();
                    return STOP_MESSAGE;
                case ADD_RULE_COMMAND:
                    //add the language rule. 
                    return await AddTranslationRule(command.Parameters);
                case LANGS_COMMAND:
                    return string.Join("\n", await yandexTranslationService.GetLanguages());
                case HELP_COMMAND:
                    return HELP_MESSAGE;
            }
            return null;
        }

        /// <summary>
        /// Detect the language of the message and then translates 
        /// it if there is a rule for the language
        /// </summary>
        /// <param name="message">The text to be translated.</param>
        /// <returns>The translated text or <c>null</c> if either the translation 
        /// failed or the text was empty or there was no translation rule for this language.</returns>
        async Task TranslateText(string message, IMessageProcessedCallback callback)
        {
            try
            {
                string sourceLang = await yandexTranslationService.DetectLanguage(message, 
                    new List<string>(translationRules.Keys));

                string targetLang = translationRules.GetValueOrDefault(sourceLang, null);

                string translation = await yandexTranslationService.
                    TranslateText(message, sourceLang, targetLang);
                await callback.OnAnswerMessageGenerated(translation);
            } catch(Exception e)
            {
                Console.WriteLine("Exception when trying to translate. message:  " +
                    message + " Exception: " + e);
                await callback.OnAnswerMessageGenerated(TRANSLATION_FAILED_MESSAGE);
            }
        }

        async Task<string> AddTranslationRule(string[] message)
        {
            string errorMessage = await ValidateLangRule(message);
            if (errorMessage != null)
                return errorMessage;
            
            //now add them as rules for translation
            translationRules.Add(message[0], message[1]);
            OnModuleChanged();
            return "Successfully added translation rule from " + message[0] + " to " + message[1];
        }
        
        async Task<string> ValidateLangRule(string[] message)
        {
            if(message.Length != 2)
            {
                return INSUFFICIENT_RULE_PARAMETERS_MESSAGE;
            }
            if (message[0] == null || message[0].Length != 2)
            {
                return message[0] + INVALID_LANG_CODE_MESSAGE;
            }
            if (message[1] == null || message[1].Length != 2)
            {
                return message[1] + INVALID_LANG_CODE_MESSAGE;
            }
            if (!await yandexTranslationService.IsTranslationRuleValid(message[0], message[1]))
                return RULE_INVALID_MESSAGE;
            return null;
        }

        public override string ToString()
        {
            return "active: " + active + " rules: " + string.Join(";", translationRules.Select(x => x.Key + "=" + x.Value).ToArray());
        }

        public void OnDeserialization(object sender)
        {
            yandexTranslationService = new YandexTranslateService();
            _MessageTypeFilter =
            new HashSet<MessageType>()
            {
                MessageType.Text
            }.ToImmutableHashSet();
        }

        [NonSerialized]
        IImmutableSet<MessageType> _MessageTypeFilter;

        public IImmutableSet<MessageType> MessageTypeFilter { get
            {
                return _MessageTypeFilter;
            }
        }

        public static IImmutableSet<UpdateType> UpdateTypeFilterStatic { get; } =
            new HashSet<UpdateType>()
            {
                UpdateType.Message
            }.ToImmutableHashSet();

        //TODO implement serialize and unserialize
    }
}