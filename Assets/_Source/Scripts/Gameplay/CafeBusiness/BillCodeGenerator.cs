using System;

namespace ITCafe.CafeBusiness
{
    public static class BillCodeGenerator
    {
        private readonly static Random Random = new();
        private const string PREFIX = "#";
        private const string CHARACTERS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GetCode(int length = 6)
        {
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = CHARACTERS[Random.Next(CHARACTERS.Length)];
            }

            return $"{PREFIX}{new string(code)}";
        }
    }
}