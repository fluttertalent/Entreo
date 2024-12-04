using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entreo.Attributes;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Services;

namespace WebApp.Entreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;
        private readonly ILogger<TranslationController> _logger;
        private readonly IUserAccessor _userAccessor;

        public TranslationController(
            ITranslationService translationService,
            ILogger<TranslationController> logger,
            IUserAccessor userAccessor)
        {
            _translationService = translationService;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        [HttpGet("{fieldName}")]
        public async Task<string> GetTranslation(string languageCode, [FromQuery] string fieldName)
        {
            try
            {
                return await _translationService.GetTranslation(fieldName, languageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get translation for field {FieldName} in {LanguageCode}",
                    fieldName, languageCode);
                throw;
            }
        }

        [HttpGet("language/{languageCode}")]
        public async Task<Dictionary<string, string>> GetLanguageTranslations(string languageCode)
        {
            try
            {
                return await _translationService.GetLanguageTranslations(languageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get translations for language {LanguageCode}", languageCode);
                throw;
            }
        }

        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Admin,Translator")]
        public async Task<Translation> CreateTranslation([FromBody] Translation translation)
        {
            try
            {
                return await _translationService.SetTranslation(
                    translation.FieldName,
                    translation.LanguageCode,
                    translation.Value,
                    translation.TranslationSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create translation for {FieldName} in {LanguageCode}",
                    translation.FieldName, translation.LanguageCode);
                throw;
            }
        }

        [HttpDelete("{fieldName}")]
        [Authorize(Roles = "Admin")]
        public async Task DeleteTranslation(string fieldName, [FromQuery] string languageCode)
        {
            try
            {
                await _translationService.DeleteTranslation(fieldName, languageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete translation for {FieldName} in {LanguageCode}",
                    fieldName, languageCode);
                throw;
            }
        }
    }
}