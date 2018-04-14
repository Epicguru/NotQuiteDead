
public class TimeCommand : Command
{
    public TimeCommand()
    {
        Name = "time";
        parameters.Add(typeof(float));
    }

    public override string Execute(object[] args)
    {
        float time = (float)args[0];

        GameTime.Instance.SetTime(time);

        return null;
    }
}