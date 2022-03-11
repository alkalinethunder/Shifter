using System.IO;
using UnityEngine;

namespace Customization.ShiftOS
{
    public static class PhilUtility
    {
        public static Vector2 ReadVector(StreamReader reader)
        {
            var xText = reader.ReadLine();
            var yText = reader.ReadLine();
            var x = float.Parse(xText);
            var y = float.Parse(yText);
            return new Vector2(x, y);
        }

        public static Vector2 ReadBackwardsVector(StreamReader reader)
        {
            var vector = ReadVector(reader);
            return new Vector2(vector.y, vector.x);
        }
        
        public static int ReadNumber(StreamReader reader)
        {
            return int.Parse(reader.ReadLine());
        }

        public static bool ReadBoolean(StreamReader reader)
        {
            return reader.ReadLine() == "11";
        }

        public static string ReadColorOrDefault(StreamReader reader, string defaultColor)
        {
            var rawColor = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(rawColor))
                return defaultColor;

            return ReadColorInternal(rawColor);
        }
        
        public static string ReadColor(StreamReader reader)
        {
            var rawColor = reader.ReadLine();

            return ReadColorInternal(rawColor);
        }

        private static string ReadColorInternal(string rawColor)
        {
            var colorData = (uint) int.Parse(rawColor);

            var b = (byte) colorData;
            var g = (byte) (colorData >> 8);
            var r = (byte) (colorData >> 16);

            return $"#{GetHtmlByte(r)}{GetHtmlByte(g)}{GetHtmlByte(b)}";
        }

        public static string GetHtmlByte(byte b)
        {
            var hex = b.ToString("X");
            if (hex.Length != 2)
                hex = "0" + hex;
            return hex;
        }
    }

    public enum TitleTextPosition
    {
        Left,
        Centre
    }
}