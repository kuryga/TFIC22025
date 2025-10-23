using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UI
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public static class InputSanitizer
    {
        public const string AllowedPattern = @"^[a-zA-Z0-9!@#$^&?_+<>.:]+$";

        public static void ProtectTextBox(TextBox textBox)
        {
            textBox.KeyPress -= TextBox_KeyPress_Filter;
            textBox.KeyPress += TextBox_KeyPress_Filter;
        }

        private static void TextBox_KeyPress_Filter(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!Regex.IsMatch(e.KeyChar.ToString(), AllowedPattern))
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
    }
}