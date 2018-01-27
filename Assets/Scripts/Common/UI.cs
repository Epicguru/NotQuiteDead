
public static class UI
{
    public static bool AnyOpen
    {
        get
        {
            return OpenCount > 0;
        }
        private set
        {

        }
    }

    public static int OpenCount { get; private set; }

    public static void FlagOpen()
    {
        OpenCount++;
    }

    public static void FlagClosed()
    {
        OpenCount--;
    }

    public static void Reset()
    {
        OpenCount = 0;
    }
}