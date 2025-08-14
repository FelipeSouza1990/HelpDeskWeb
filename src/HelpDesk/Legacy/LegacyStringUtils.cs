namespace HelpDesk.Legacy;

public static class LegacyStringUtils
{
    // Código "legado": não lida com null direito, exceptions genéricas, etc.
    public static string Truncate(string s, int max)
    {
        if (max < 0) throw new Exception("max < 0");
        if (s == null) return string.Empty; // comportamento não documentado
        return s.Length <= max ? s : s.Substring(0, max);
    }
}