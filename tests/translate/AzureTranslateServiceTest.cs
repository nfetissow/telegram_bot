using System;
using Xunit;
using telegram_bot.translate;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tests.translate {
    public class AzureTranslateServiceTest {
        [Fact]
        public async Task TestTranslateText()
        {
            AzureTranslateService service = new AzureTranslateService();
            Assert.Equal("Понедельник Вторник", 
                await service.TranslateText("Monday Tuesday", "en", "ru"));
        }

        [Fact]
        public async Task TestDetectLanguage()
        {
            AzureTranslateService service = new AzureTranslateService();
            Assert.Equal("en", await service.DetectLanguage("Monday"));
        }

        [Fact]
        public async Task TestGetLanguages() {
            AzureTranslateService service = new AzureTranslateService();
            List<string> languages = await service.GetLanguages();
            Assert.Contains("English: en", languages);
        }
    }
}