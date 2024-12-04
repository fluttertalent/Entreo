using Caching;
using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Data;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Services
{

    public interface IPostalCodeService
    {
        Task<string?> GetPostalCodeByPosition(double lat1, double lon1);
        Task<string?> GetCityByPostalCode(string? postalCode);
    }

    public class PostalCodeService : IPostalCodeService
    {
        private readonly ApplicationDbContext _dbContext;

        public PostalCodeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string?> GetPostalCodeByPosition(double latitude, double longitude)
        {
            var postalCodes = await CacheUtility.Get(nameof(PostalCode), nameof(PostalCode), async () => await _dbContext.PostalCodes.ToListAsync());

            double increment = 0.1;
            List<PostalCode> subset = new List<PostalCode>();

            while (subset.Count == 0)
            {
                subset = postalCodes.Where(p => p.Latitude >= latitude - increment && p.Latitude < latitude + increment &&
                                                     p.Longitude >= longitude - increment && p.Longitude < longitude + increment).ToList();
            }
            subset = [.. subset.OrderBy(p => GetDistance(p.Latitude, p.Longitude, latitude, longitude))];
            var bestGuess = subset.FirstOrDefault();

            return bestGuess?.PoststalCode;
        }

        public async Task<string?> GetCityByPostalCode(string? postalCode)
        {
            if (string.IsNullOrEmpty(postalCode))
                return string.Empty;

            var postalCodes = await CacheUtility.Get(nameof(PostalCode), nameof(PostalCode), async () => await _dbContext.PostalCodes.ToListAsync());
            string? city = postalCodes.FirstOrDefault(p => p.PoststalCode == postalCode)?.City;

            return city;
        }

        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in km
            double lat1Rad = DegreesToRadians(lat1);
            double lon1Rad = DegreesToRadians(lon1);
            double lat2Rad = DegreesToRadians(lat2);
            double lon2Rad = DegreesToRadians(lon2);

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance; // Distance in km
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}