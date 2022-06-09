namespace Socolin.TestUtils.Console;

public class AnsiColorCodes
{
    public const string Reset = $"{EscapeCode}[0m";
    private const string EscapeCode = "\u001b";

    public static string Color(Terminal8ColorCodes code)
    {
        return $"{EscapeCode}[" + (int)code + "m";
    }

    public static string Color(string text, Terminal8ColorCodes code)
    {
        return Color(code) + text + Reset;
    }

    public static string Color256(Terminal256ColorCodes code)
    {
        return $"{EscapeCode}[38;5;{(int)code}m";
    }

    public static string Color256(string text, Terminal256ColorCodes code)
    {
        return $"{EscapeCode}[38;5;{(int)code}m" + text + Reset;
    }
}