using System;
namespace pfDataSource.Common
{
	public static class Extensions
	{
		public static string ToHexString(this byte[] bytes)
		{
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

		public static byte[] FromHexString(this string str)
		{
            int NumberChars = str.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
            return bytes;
        }
	}
}

