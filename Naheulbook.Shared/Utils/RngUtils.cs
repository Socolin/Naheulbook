using System.Security.Cryptography;
using System.Text;

namespace Naheulbook.Shared.Utils
{
    public class RngUtils
    {
        public static string GetRandomHexString(int byteCount)
        {
            var bytes = new byte[byteCount];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static string GetRandomString(string prefix)
        {
            return prefix + GetRandomHexString(8);
        }

        public static int GetRandomInt()
        {
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];
        }
    }
}