using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dangl.BCF.Converter
{
    public static class ConversionExtensions
    {
        public static string ToRgbHexColorString(this byte[] src)
        {
            if (src?.Any() != true)
            {
                return null;
            }

            return BitConverter.ToString(src).Replace("-", string.Empty);
        }

        public static byte[] ToByteArrayFromHexRgbColor(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return null;
            }

            var isValidHexExpression = Regex.IsMatch(src, "^[0-9a-fA-F]{6}([0-9a-fA-F]{2})?$");
            if (!isValidHexExpression)
            {
                return null;
            }

            var bytes = src.Length == 6
                ? new byte[3]
                : new byte[4];

            bytes[0] = Convert.ToByte(src.Substring(0, 2), 16);
            bytes[1] = Convert.ToByte(src.Substring(2, 2), 16);
            bytes[2] = Convert.ToByte(src.Substring(4, 2), 16);

            if (bytes.Length == 4)
            {
                bytes[3] = Convert.ToByte(src.Substring(6), 16);
            }

            return bytes;
        }
    }
}
