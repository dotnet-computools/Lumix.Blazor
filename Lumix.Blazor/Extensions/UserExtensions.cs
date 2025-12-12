namespace Lumix.Blazor.Extensions
{
    public static class UserExtensions
    {
        public static string GetInitials(this string fullName, string alt)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return alt;

            var split = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length > 1 && split[0].Length > 0 && split.Last().Length > 0)
            {
                var first = split.First().ToUpper()[0];
                var last = split.Last().ToUpper()[0];
                return $"{first}{last}";
            }

            if (split.Length == 1 && split[0].Length > 0)
            {
                return $"{split.First().ToUpper()[0]}";
            }

            return alt;
        }
    }
}
