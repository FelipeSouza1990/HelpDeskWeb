namespace HelpDesk.Legacy;

public static class StringUtils
{
    public static string Truncate(string? s, int max)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(max);
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s!.Length <= max ? s : s[..max];
    }
}

//Refatoramento moderno do LegacyStringUtils para comparação