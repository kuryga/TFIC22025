using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UI
{
    public static class InputSanitizer
    {
        public const string AllowedPattern = @"^[a-zA-Z0-9!@#$^&?_+<>.:]+$";
        public const string AllowedPatternWithSpaces = @"^[a-zA-Z0-9 !@#$^&?_+<>.:]+$";

        public static void ProtectTextBox(TextBox textBox)
        {
            textBox.KeyPress -= TextBox_KeyPress_Filter;
            textBox.KeyPress += TextBox_KeyPress_Filter;
        }

        private static void TextBox_KeyPress_Filter(object sender, KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == '\r' || e.KeyChar == '\n')
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
                return;
            }

            var tb = sender as TextBox;
            var tag = (tb != null ? tb.Tag as string : null)?.Trim();
            var isSafe = string.Equals(tag, "SAFE", StringComparison.OrdinalIgnoreCase);


            var pattern = isSafe ? AllowedPatternWithSpaces : AllowedPattern;

            if (e.KeyChar == '\r' || e.KeyChar == '\n')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
                return;
            }


            if (!Regex.IsMatch(e.KeyChar.ToString(), pattern))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public static void ProtectAllTextBoxes(Control root)
        {
            foreach (Control c in root.Controls)
            {
                if (c is TextBox tb)
                {
                    if (!(tb.Tag is string tag && tag.Equals("NoSanitize", StringComparison.OrdinalIgnoreCase)))
                        ProtectTextBox(tb);
                }
                if (c.HasChildren)
                    ProtectAllTextBoxes(c);
            }
        }

        public static bool TryNormalizeArPhoneToE164(string input, out string e164)
        {
            e164 = null;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string cleaned = input.Trim();
            if (cleaned.StartsWith("00")) cleaned = "+" + cleaned.Substring(2);
            cleaned = Regex.Replace(cleaned, @"[\s\-().]", "");

            bool hadPlus = cleaned.StartsWith("+");
            string digits = new string(cleaned.Where(char.IsDigit).ToArray());

            bool hasCountry54 = false;
            if (hadPlus && digits.StartsWith("54")) { hasCountry54 = true; digits = digits.Substring(2); }
            else if (!hadPlus && digits.StartsWith("54")) { hasCountry54 = true; digits = digits.Substring(2); }

            if (!hasCountry54 && digits.StartsWith("0"))
                digits = digits.Substring(1);

            bool isMobile = false;
            if (hasCountry54)
            {
                if (digits.StartsWith("9")) { isMobile = true; digits = digits.Substring(1); }
            }
            else
            {
                if (digits.StartsWith("15")) { isMobile = true; digits = digits.Substring(2); }
            }

            if (!hasCountry54 && !isMobile && digits.Length == 8 && digits.All(char.IsDigit))
            {
                e164 = digits;
                return true;
            }

            if (digits.Length != 10 || !digits.All(char.IsDigit))
                return false;

            e164 = isMobile ? $"+549{digits}" : $"+54{digits}";
            return true;
        }

        public static bool IsValidArPhone(string input) =>
            TryNormalizeArPhoneToE164(input, out _);

        public static bool IsValidNumeric(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input.Trim(), @"^\d{1,12}$");
        }

        public static bool IsValidUrbansoftEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input.Trim(), @"^[a-z0-9._%+-]+@urbansoft\.com$", RegexOptions.IgnoreCase);
        }

        public static bool IsSafeForSql(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            if (input.Contains("\n") || input.Contains("\r"))
                return false;

            string forbiddenPattern = @"['"";\\/*]|--|\b(ALTER|DROP|DELETE|INSERT|UPDATE|EXEC|UNION|SELECT)\b";
            return !Regex.IsMatch(input, forbiddenPattern, RegexOptions.IgnoreCase);
        }
    }
}
