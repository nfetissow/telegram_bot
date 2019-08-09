using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Linq;

namespace telegram_bot.translate
{
    public class YandexTranslateService {
        private static readonly HttpClient client = new HttpClient() {
            BaseAddress = new System.Uri("https://translate.yandex.net/api/v1.5/tr.json/")
        };

        #region Translate text

        const string TRANSLATE_TEXT_PATH = "translate";

        public async Task<string> TranslateText(string text, string sourceLang, string targetLang) {
            Dictionary<string, string> pars = GetParameterDict();
            pars.Add("text", text);
            pars.Add("lang", sourceLang + "-" + targetLang);
            pars.Add("format", "plain");
            var queryString = QueryHelpers.AddQueryString(TRANSLATE_TEXT_PATH, pars);
            var resultString = await client.GetStringAsync(queryString);
            var result = JsonConvert.DeserializeObject<TranslateTextResponse>(resultString);
            return AppendYandexText(result.text[0]);
        }

        string AppendYandexText(string translation)
        {
            return translation + "\nPowered by [Yandex.translate](http://translate.yandex.com/).";
        }

        class TranslateTextResponse
        {
            public string code;
            public string lang;
            public string[] text;
        }

        #endregion

        #region Detect language

        const string DETECT_LANGUAGE_PATH = "detect";

        public async Task<string> DetectLanguage(string text, List<string> langHints = null) {
            Dictionary<string, string> pars = GetParameterDict();
            pars.Add("text", text);
            if(langHints != null && langHints.Count > 0)
                pars.Add("hint", string.Join(",", langHints));
            var queryString = QueryHelpers.AddQueryString(DETECT_LANGUAGE_PATH, pars);
            var resultString = await client.GetStringAsync(queryString);
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultString);
            return result["lang"];
        }

        #endregion

        #region Get languages

        const string GET_LANGUAGE_PATH = "getLangs";

        public async Task<List<string>> GetLanguages() {
            var langsDict = (await GetLangsFromServer()).langs;
            List<string> readableList = new List<string>();
            foreach (KeyValuePair<string, string> entry in langsDict)
            {
                readableList.Add(entry.Value + ": " + entry.Key);
            }
            return readableList;
        }

        public async Task<bool> IsTranslationRuleValid(string sourceLang, string targetLang)
        {
            string asRule = sourceLang + "-" + targetLang;
            string[] validRules = (await GetLangsFromServer()).dirs;
            return validRules.Contains(asRule);
        }

        class GetLanguagesResponse{
            /// <summary>
            /// Contains all possible translation directions in form en-ru
            /// </summary>
            public string[] dirs;
            /// <summary>
            /// Contains pairs of languagecode, displayname of language e.g. en English
            /// </summary>
            public Dictionary<string, string> langs;
        }

        async Task<GetLanguagesResponse> GetLangsFromServer()
        {
            Dictionary<string, string> pars = GetParameterDict();
            pars.Add("ui", "en");
            var queryString = QueryHelpers.AddQueryString(GET_LANGUAGE_PATH, pars);
            var resultString = await client.GetStringAsync(queryString);
            return JsonConvert.DeserializeObject<GetLanguagesResponse>(resultString);
        }

        #endregion

        static Dictionary<string, string> GetParameterDict() {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("key", APIKeys.YANDEX_KEY);
            return pars;
        }
    }
}