namespace Naheulbook.Shared.Utils;

public interface IRngUtil
{
    string GetRandomHexString(int byteCount);
    string GetRandomString(string prefix);
    int GetRandomInt();
    int GetRandomInt(int min, int max);
}

public class RngUtil : IRngUtil
{
    public string GetRandomHexString(int byteCount)
    {
        return RngHelper.GetRandomHexString(byteCount);
    }

    public string GetRandomString(string prefix)
    {
        return RngHelper.GetRandomString(prefix);
    }

    public int GetRandomInt()
    {
        return RngHelper.GetRandomInt();
    }

    public int GetRandomInt(int min, int max)
    {
        return RngHelper.GetRandomInt(min, max);
    }
}