using System.Text.RegularExpressions;

namespace GenerateHtml.ClassHelpers
{
    public static class ChangeSymbol
    {
        public static string DoChange(string input)
        {
            string[] specWrd = {"&quot;", "'", "%", ".", ";", "/", "\"", ":", "#", "+", "*", "&amp;", "?", "æ", "ø", "å", "ä", "ö", "ü", "ß", "Ä", "Ö", "&lt;", "&gt;" };
            string[] specWrd1 = { "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ" };
            string[] specWrd2 = { "ý", "ỳ", "ỷ", "ỹ", "ỵ" };
            string[] specWrd3 = { "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự" };
            string[] specWrd4 = { "í", "ì", "ỉ", "ĩ", "ị" };
            string[] specWrd5 = { "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ" };
            string[] specWrd6 = { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ" };
            string[] specWrd7 = { "đ" };
            string[] specWrd8 = { " ", "|" };
            input = input.ToLower();
            foreach (var s in specWrd)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "");
                }
            }
            foreach (var s in specWrd1)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "e");
                }
            }
            foreach (var s in specWrd2)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "y");
                }
            }
            foreach (var s in specWrd3)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "u");
                }
            }
            foreach (var s in specWrd4)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "i");
                }
            }
            foreach (var s in specWrd5)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "o");
                }
            }
            foreach (var s in specWrd6)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "a");
                }
            }
            foreach (var s in specWrd7)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "d");
                }
            }
            foreach (var s in specWrd8)
            {
                if (input.Contains(s))
                {
                    input = input.Replace(s, "-");
                }
            }
            input = Regex.Replace(input, @"\r\n?|\n", "");
            return input;
        }
    }
}