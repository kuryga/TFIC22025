using System;
using System.Linq;
using System.Windows.Forms;

namespace WinApp
{

    public static class PhoneValidator
    {
        private static readonly char[] AllowedChars = "0123456789+ -()".ToCharArray();
        public static int MaxDigitsLocal = 11;
        public static int MaxDigitsInternational = 13;

        public static void Attach(TextBox textBox, ErrorProvider errorProvider = null)
        {
            textBox.KeyPress -= OnKeyPress;
            textBox.KeyPress += OnKeyPress;

            textBox.TextChanged -= OnTextChanged;
            textBox.TextChanged += OnTextChanged;

            textBox.Validating -= OnValidating;
            textBox.Validating += OnValidating;

            void OnValidating(object sender, System.ComponentModel.CancelEventArgs e)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    errorProvider?.SetError(textBox, "");
                    return;
                }

                if (!InputSanitizer.TryNormalizeArPhoneToE164(textBox.Text, out var e164))
                {
                    e.Cancel = true;
                    errorProvider?.SetError(textBox, "Teléfono argentino inválido");
                }
                else
                {
                    textBox.AccessibleDescription = e164;
                    errorProvider?.SetError(textBox, "");
                }
            }

            void OnKeyPress(object s, KeyPressEventArgs e)
            {
                if (char.IsControl(e.KeyChar)) return;
                char ch = e.KeyChar;

                if (!AllowedChars.Contains(ch))
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                    return;
                }

                if (ch == '+')
                {
                    if (textBox.Text.Contains('+') || textBox.SelectionStart != 0)
                    {
                        e.Handled = true;
                        System.Media.SystemSounds.Beep.Play();
                    }
                    return;
                }

                if (char.IsDigit(ch))
                {
                    int existingDigits = CountDigits(textBox.Text);
                    int selectedDigits = CountDigits(textBox.SelectedText);
                    int projectedDigits = existingDigits - selectedDigits + 1;

                    bool isInternational = textBox.Text.StartsWith("+") || textBox.Text.StartsWith("00");
                    int limit = isInternational ? MaxDigitsInternational : MaxDigitsLocal;

                    if (projectedDigits > limit)
                    {
                        e.Handled = true;
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
            }

            void OnTextChanged(object s, EventArgs e)
            {
                int caret = textBox.SelectionStart;
                string raw = textBox.Text;

                if (raw.StartsWith("00")) raw = "+" + raw.Substring(2);

                string cleaned = new string(raw.Where(c => AllowedChars.Contains(c)).ToArray());

                int plusCount = cleaned.Count(c => c == '+');
                if (plusCount > 0)
                {
                    cleaned = cleaned.Replace("+", "");
                    cleaned = "+" + cleaned.TrimStart();
                }

                bool isInternational = cleaned.StartsWith("+");
                int limit = isInternational ? MaxDigitsInternational : MaxDigitsLocal;
                cleaned = TrimToDigitLimit(cleaned, limit);

                if (cleaned != textBox.Text)
                {
                    textBox.TextChanged -= OnTextChanged;
                    textBox.Text = cleaned;
                    textBox.SelectionStart = Math.Min(caret, cleaned.Length);
                    textBox.TextChanged += OnTextChanged;
                }
            }
        }

        private static int CountDigits(string s) => s?.Count(char.IsDigit) ?? 0;

        private static string TrimToDigitLimit(string s, int maxDigits)
        {
            if (CountDigits(s) <= maxDigits) return s;

            char[] arr = s.ToCharArray();
            int digits = CountDigits(s);
            for (int i = arr.Length - 1; i >= 0 && digits > maxDigits; i--)
            {
                if (char.IsDigit(arr[i]))
                {
                    arr[i] = '\0';
                    digits--;
                }
            }
            return new string(arr.Where(c => c != '\0').ToArray());
        }
    }
}
