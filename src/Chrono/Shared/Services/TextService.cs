namespace Chrono.Shared.Services;

public static class TextService
{
    public static string Truncate(string text, int length)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= length)
        {
            return text;
        }

        return text[..(length - 3)] + "...";
    }
}
