using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CltOnekey
{
    class Util
    {
        public static string RemoveInvalidCharacters(string text)
        {
            StringBuilder titleBuilder = new StringBuilder(text);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            {
                titleBuilder = titleBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            }
            return titleBuilder.ToString();
        }
    }
}
