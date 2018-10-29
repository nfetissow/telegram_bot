using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Linq;

namespace telegram_bot.translate {
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
            return result.text[0];
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
            Dictionary<string, string> pars = GetParameterDict();
            pars.Add("ui", "en");
            var queryString = QueryHelpers.AddQueryString(GET_LANGUAGE_PATH, pars);
            var resultString = await client.GetStringAsync(queryString);
            var result = JsonConvert.DeserializeObject<GetLanguagesResponse>(resultString);
            return result.GetReadableLanguageList();
        }

        class GetLanguagesResponse{
            public string[] dirs;
            public Dictionary<string, string> langs;

            public List<string> GetReadableLanguageList() {
                List<string> readableList = new List<string>();
                foreach(KeyValuePair<string, string> entry in langs) {
                    readableList.Add(entry.Value + ": " + entry.Key);
                }
                return readableList;
            }
        }

        #endregion

        static Dictionary<string, string> GetParameterDict() {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("key", APIKeys.YANDEX_KEY);
            return pars;
        }
    }
}