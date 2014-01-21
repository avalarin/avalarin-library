using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tests {
    public static class StringUtility {
        public static string RandomString(Random random, int size) {
            if (random == null) throw new ArgumentNullException("random");
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++) {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static string GetPasswordHash(string password, string salt) {
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(password + salt);
            var hash = md5.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        public static string Format(this string format, params object[] args) {
            return string.Format(format, args);
        }

        public static string Format(this string format, IFormatProvider provider, params object[] args) {
            return string.Format(provider, format, args);
        }

        public static string BytesToString(long byteCount) {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        public static string UpperFirstLetterInvariant(this string str) {
            return UpperFirstLetter(str, CultureInfo.InvariantCulture);
        }
        
        public static string UpperFirstLetter(this string str) {
            return UpperFirstLetter(str, CultureInfo.CurrentCulture);
        }
        
        public static string UpperFirstLetter(this string str, CultureInfo culture) {
            if (str == null) throw new ArgumentNullException("str");
            if (str.Length == 0) { return str; }
            if (str.Length == 1) { return str.ToUpper(culture); }
            var chars = str.ToCharArray();
            chars[0] = Char.ToUpper(chars[0], culture);
            return new string(chars);
        }

        public static string ReplaceChars(string str, char[] chars, char replacement) {
            if (str == null) throw new ArgumentNullException("str");
            var newChars = str.Select(c => {
                if (chars.Contains(c)) {
                    return replacement;
                }
                return c;
            }).ToArray();
            return new string(newChars);

        }

        public static IEnumerable<string> CreateStrings(string baseString, Func<string, int, string> change) {
            if (baseString == null) throw new ArgumentNullException("baseString");
            if (change == null) throw new ArgumentNullException("changeString");
            var index = 1;
            var currentString = baseString;
            yield return baseString;
            while (true) {
                currentString = change(currentString, index++);
                yield return currentString;
            }
        }
    }
}