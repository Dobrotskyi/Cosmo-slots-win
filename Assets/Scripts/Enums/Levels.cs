public enum Levels
{
    First,
    Second,
    Third
}

public static class LevelsExtension
{
    public static Levels GetNext(this Levels level)
    {
        if (level == Levels.First)
            return Levels.Second;

        return Levels.Third;
    }

    public static Levels GetPrev(this Levels level)
    {
        if (level == Levels.Third)
            return Levels.Second;

        return Levels.First;
    }
}