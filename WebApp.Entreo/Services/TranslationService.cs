using Caching;
using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Data;

namespace WebApp.Entreo.Services
{
    public interface ITranslationService
    {
        Task<string> GetTranslation(string fieldName, string languageCode);
        Task<Dictionary<string, string>> GetLanguageTranslations(string languageCode);
        Task<Translation> SetTranslation(string fieldName, string languageCode, string value, string translationSource);
        Task DeleteTranslation(string fieldName, string languageCode);
    }

    public class TranslationService : ITranslationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TranslationService> _logger;
        private const string CACHE_KEY_PREFIX = "translations";
        private const string CACHE_GROUP = "translations";

        public TranslationService(
            ApplicationDbContext context,
            ILogger<TranslationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GetTranslation(string fieldName, string languageCode)
        {
            // Get translations from cached language dictionary
            var translations = await GetLanguageTranslations(languageCode);

            if (translations.TryGetValue(fieldName, out string translation))
            {
                return translation;
            }

            _logger.LogInformation("Translation not found for field {FieldName} in {LanguageCode}",
                fieldName, languageCode);
            return fieldName; // Return original field name as fallback
        }

        public async Task<Dictionary<string, string>> GetLanguageTranslations(string languageCode)
        {
            return await CacheUtility.Get($"{CACHE_KEY_PREFIX}:{languageCode}", CACHE_GROUP, () =>
            {
                return _context.Translations
                    .AsNoTracking()
                    .Where(t => t.LanguageCode == languageCode)
                    .ToDictionaryAsync(
                        t => t.FieldName,
                        t => t.Value
                    );
            });
        }

        public async Task<Translation> SetTranslation(
            string fieldName,
            string languageCode,
            string value,
            string translationSource)
        {
            var translation = await _context.Translations
                .FirstOrDefaultAsync(t =>
                    t.FieldName == fieldName &&
                    t.LanguageCode == languageCode);

            var now = DateTime.UtcNow;

            if (translation == null)
            {
                translation = new Translation
                {
                    FieldName = fieldName,
                    LanguageCode = languageCode,
                    Value = value,
                    TranslationSource = translationSource,
                    CreatedAt = now
                };
                _context.Translations.Add(translation);
            }
            else
            {
                translation.Value = value;
                translation.TranslationSource = translationSource;
                translation.UpdatedAt = now;
            }

            await _context.SaveChangesAsync();

            // Invalidate language cache
            InvalidateLanguageCache(languageCode);

            return translation;
        }

        public async Task DeleteTranslation(string fieldName, string languageCode)
        {
            var translation = await _context.Translations
                .FirstOrDefaultAsync(t =>
                    t.FieldName == fieldName &&
                    t.LanguageCode == languageCode);

            if (translation != null)
            {
                _context.Translations.Remove(translation);
                await _context.SaveChangesAsync();

                // Invalidate language cache
                InvalidateLanguageCache(languageCode);
            }
        }

        private void InvalidateLanguageCache(string languageCode)
        {
            CacheUtility.Remove($"{CACHE_KEY_PREFIX}:{languageCode}", CACHE_GROUP);
        }
    }
}