using Xunit;
using telegram_bot.translate;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tests.translate
{
    public class YandexTranslateServiceTest {
        [Fact]
        public async Task TestTranslateText()
        {
            YandexTranslateService service = new YandexTranslateService();
            Assert.Equal("Понедельник Вторник\nPowered by Yandex translate.", 
                await service.TranslateText("Monday Tuesday", "en", "ru"));
        }

        [Fact]
        public async Task TestDetectLanguage()
        {
            YandexTranslateService service = new YandexTranslateService();
            Assert.Equal("en", await service.DetectLanguage("Monday"));
        }

        [Fact]
        public async Task TestGetLanguages() {
            YandexTranslateService service = new YandexTranslateService();
            List<string> languages = await service.GetLanguages();
            Assert.Contains("English: en", languages);
        }

        [Fact]
        public async Task TestValidateRule()
        {
            YandexTranslateService service = new YandexTranslateService();
            Assert.True(await service.IsTranslationRuleValid("en", "ru"));
            Assert.False(await service.IsTranslationRuleValid("ru", "ru"));
        }
    }
}