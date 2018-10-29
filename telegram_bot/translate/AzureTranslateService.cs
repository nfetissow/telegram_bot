using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace telegram_bot.translate
{
    public class AzureTranslateService
    {
        private static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new System.Uri("")
        };

        #region Translate text

        public async Task<string> TranslateText(string text, string sourceLang, string targetLang)
        {
            return await client.GetStringAsync("");
        }

        #endregion

        #region Detect language

        public async Task<string> DetectLanguage(string text, List<string> langHints = null)
        {
            return await client.GetStringAsync("");
        }

        #endregion

        #region Get languages

        public async Task<List<string>> GetLanguages()
        {
            return new List<string>()
            {
                await client.GetStringAsync("")
            };
        }

        #endregion

        static Dictionary<string, string> GetParameterDict()
        {
            Dictionary<string, string> pars = new Dictionary<string, string>();
            pars.Add("key", APIKeys.AZURE_KEY);
            return pars;
        }
    }
}