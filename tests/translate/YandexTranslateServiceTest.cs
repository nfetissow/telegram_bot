using System;
using Xunit;
using telegram_bot.translate;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tests.translate {
    public class YandexTranslateServiceTest {
        [Fact]
        public async Task TestTranslateText()
        {
            YandexTranslateService service = new YandexTranslateService();
            Assert.Equal("Понедельник Вторник", 
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
    }
}