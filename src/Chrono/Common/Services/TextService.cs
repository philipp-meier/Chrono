namespace Chrono.Common.Services;

public class TextService
{
    public string Truncate(string text, int length)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= length)
        {
            return text;
        }

        return text[..(length - 3)] + "...";
    }
}
