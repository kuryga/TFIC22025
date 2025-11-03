using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinApp
{
    public static class TextBoxTag
    {
        public const string Num12 = "NUM_12";
        public const string MailUrban = "MAIL_URBANSOFT";
        public const string SqlSafe = "SAFE";
        public const string Pwd = "PASSWORD";
        public const string PwdVerify = "VERIFY_PASS";
        public const string PhoneNumber = "AR_PHONE";
        public const string Price = "PRICE";
        public const string NoSanitize = "NoSanitize";
    }

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

            if (string.Equals(tag, TextBoxTag.Price, StringComparison.OrdinalIgnoreCase))
            {
                var decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var text = tb.Text ?? string.Empty;

                // permitir solo dígitos y un separador decimal
                if (char.IsDigit(e.KeyChar))
                {
                    var sepIndex = text.IndexOf(decSep);
                    if (sepIndex >= 0)
                    {
                        var decimalsCount = text.Length - sepIndex - 1;
                        var cursorPos = tb.SelectionStart;
                        if (cursorPos > sepIndex && decimalsCount >= 2)
                        {
                            e.Handled = true;
                            System.Media.SystemSounds.Beep.Play();
                            return;
                        }
                    }
                    return;
                }

                if (e.KeyChar == '.' || e.KeyChar == ',')
                {
                    var hasSep = text.Contains(".") || text.Contains(",");
                    if (hasSep)
                    {
                        e.Handled = true;
                        System.Media.SystemSounds.Beep.Play();
                        return;
                    }

                    if (e.KeyChar.ToString() != decSep)
                    {
                        int sel = tb.SelectionStart;
                        tb.Text = text.Insert(sel, decSep);
                        tb.SelectionStart = sel + decSep.Length;
                        e.Handled = true;
                        return;
                    }

                    return;
                }

                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            var isSafe = string.Equals(tag, TextBoxTag.SqlSafe, StringComparison.OrdinalIgnoreCase);
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
                    if (!(tb.Tag is string tag && tag.Equals(TextBoxTag.NoSanitize, StringComparison.OrdinalIgnoreCase)))
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

        public static bool IsValidNewPassword(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (input.Length < 8)
                return false;

            if (!input.Any(char.IsUpper))
                return false;

            if (!Regex.IsMatch(input, @"[!@#$^&?_+<>.:]"))
                return false;

            if (Regex.Matches(input, @"\d").Count < 2)
                return false;

            if (!IsSafeForSql(input))
                return false;

            return true;
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

        public static bool TryParsePrice(string input, out decimal price)
        {
            price = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var s = input.Trim();
            var styles = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            var cultures = new CultureInfo[]
            {
                CultureInfo.CurrentCulture,
                CultureInfo.InvariantCulture,
                new CultureInfo("es-AR")
            };

            for (int i = 0; i < cultures.Length; i++)
            {
                var c = cultures[i];
                if (decimal.TryParse(s, styles, c, out price))
                {
                    price = Math.Round(price, 2, MidpointRounding.AwayFromZero);
                    return true;
                }
            }

            s = s.Replace(" ", "").Replace(".", "").Replace(",", ".");
            var success = decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
            if (success)
                price = Math.Round(price, 2, MidpointRounding.AwayFromZero);
            return success;
        }

        public static bool IsValidPrice(string input, int maxIntegerDigits = 12)
        {
            decimal val;
            if (!TryParsePrice(input, out val)) return false;

            var abs = Math.Abs(val);
            var integerPart = (long)Math.Truncate(abs);
            return integerPart.ToString(CultureInfo.InvariantCulture).Length <= maxIntegerDigits;
        }

        public static string FormatPrice2(string input, CultureInfo culture, bool includeCurrencySymbol)
        {
            decimal val;
            if (!TryParsePrice(input, out val)) return "0.00";

            if (culture == null) culture = CultureInfo.CurrentCulture;

            return includeCurrencySymbol
                ? val.ToString("C2", culture)
                : val.ToString("N2", culture);
        }
    }
}
