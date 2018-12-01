using System;
using System.Security.Cryptography;
using System.Text;

namespace Naheulbook.Tests.Functional.Code.Utils
{
    public static class RngUtils
    {
        public static string GetRandomHexString(int byteCount)
        {
            Span<byte> bytes = stackalloc byte[byteCount];
            RandomNumberGenerator.Fill(bytes);
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}