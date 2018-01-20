
public class ThrowableAnimationCallbacks : ItemAnimationCallback
{
    private Throwable t;

    // The strings are so that they don't show in the editor, [HideInInspector] does not work here.
    private Throwable T(string a = null, string b = null)
    {
        if (t == null)
            t = GetComponentInParent<Throwable>();

        return t;
    }

    public void EquipEnd()
    {
        T().Anim_DoneEquipping();
    }

    public void ReadyToThrow()
    {
        T().Anim_CanNowThrow();
    }

    public void Throw()
    {
        T().Anim_Throw();
    }

    public void ThrowDone()
    {
        T().Anim_Done();
    }
}