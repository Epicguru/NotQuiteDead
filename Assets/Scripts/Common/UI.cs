
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
        Verify();
    }

    public static void FlagClosed()
    {
        OpenCount--;
        Verify();
    }

    public static void Reset()
    {
        OpenCount = 0;
        Verify();
    }

    private static void Verify()
    {
        if (OpenCount < 0)
            OpenCount = 0;
    }
}