namespace WebApp.Entreo.Models.Enums
{
    public static class MoodRatingExtensions
    {
        public static string GetDescription(this MoodRating rating)
        {
            return rating switch
            {
                MoodRating.VeryNegative => "Feeling extremely down or distressed",
                MoodRating.Negative => "Feeling down or unhappy",
                MoodRating.SlightlyNegative => "Feeling slightly below neutral",
                MoodRating.Neutral => "Feeling balanced and neutral",
                MoodRating.SlightlyPositive => "Feeling slightly upbeat",
                MoodRating.Positive => "Feeling good and content",
                MoodRating.VeryPositive => "Feeling excellent and energized",
                _ => "Unknown mood state"
            };
        }

        public static string GetEmoji(this MoodRating rating)
        {
            return rating switch
            {
                MoodRating.VeryNegative => "😢",
                MoodRating.Negative => "😕",
                MoodRating.SlightlyNegative => "🙁",
                MoodRating.Neutral => "😐",
                MoodRating.SlightlyPositive => "🙂",
                MoodRating.Positive => "😊",
                MoodRating.VeryPositive => "😄",
                _ => "❓"
            };
        }

        public static string GetColor(this MoodRating rating)
        {
            return rating switch
            {
                MoodRating.VeryNegative => "#FF4136",  // Red
                MoodRating.Negative => "#FF851B",      // Orange
                MoodRating.SlightlyNegative => "#FFDC00", // Yellow
                MoodRating.Neutral => "#FFFFFF",       // White
                MoodRating.SlightlyPositive => "#A8E6CF", // Light Green
                MoodRating.Positive => "#3D9970",      // Green
                MoodRating.VeryPositive => "#2ECC40",  // Bright Green
                _ => "#AAAAAA"                         // Gray
            };
        }

        public static bool IsPositive(this MoodRating rating)
        {
            return rating >= MoodRating.SlightlyPositive;
        }

        public static bool IsNegative(this MoodRating rating)
        {
            return rating <= MoodRating.SlightlyNegative;
        }
    }
}