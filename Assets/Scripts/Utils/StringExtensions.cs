
public static class StringExtensions
{
    public static string Form(this string str, params object[] args)
    {
        return string.Format(str, args);
    }
}