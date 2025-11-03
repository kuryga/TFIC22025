using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public class BaseForm : Form
    {
        public bool EnableSanitization { get; set; } = true;
        public bool ConfigureCombosAsDropDownList { get; set; } = true;
        public bool LockNumericUpDownTextBox { get; set; } = true;
        public bool EnableArPhoneValidation { get; set; } = true;

        private readonly ErrorProvider _sharedErrorProvider;

        public BaseForm()
        {
            _sharedErrorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyPoliciesRecursive(this);

            this.ControlAdded -= BaseForm_ControlAdded;
            this.ControlAdded += BaseForm_ControlAdded;
        }

        private void BaseForm_ControlAdded(object sender, ControlEventArgs e)
        {
            ApplyPoliciesRecursive(e.Control);
        }

        private void ApplyPoliciesRecursive(Control root)
        {
            foreach (Control c in root.Controls)
            {
                var tb = c as TextBox;
                if (tb != null)
                {
                    var tag = tb.Tag as string;
                    var tagUpper = string.IsNullOrWhiteSpace(tag) ? null : tag.Trim().ToUpperInvariant();
                    if (EnableSanitization)
                    {
                        if (!(tb.Tag is string tagSan && tagSan.Equals(TextBoxTag.NoSanitize, StringComparison.OrdinalIgnoreCase)))
                            InputSanitizer.ProtectTextBox(tb);
                    }

                    if (!string.IsNullOrEmpty(tagUpper))
                    {
                        if (EnableArPhoneValidation && string.Equals(tagUpper, TextBoxTag.PhoneNumber, StringComparison.Ordinal))
                            PhoneValidator.Attach(tb, _sharedErrorProvider);

                        if (string.Equals(tagUpper, TextBoxTag.Num12, StringComparison.Ordinal))
                            AttachNumeric12Validation(tb);

                        if (string.Equals(tagUpper, TextBoxTag.MailUrban, StringComparison.Ordinal))
                            AttachUrbansoftEmailValidation(tb);

                        if (string.Equals(tagUpper, TextBoxTag.SqlSafe, StringComparison.Ordinal))
                            AttachSafeSqlValidation(tb);

                        if (string.Equals(tagUpper, TextBoxTag.Pwd, StringComparison.Ordinal))
                        {
                            tb.UseSystemPasswordChar = true;
                            AttachSafeSqlValidation(tb);
                        }

                        if (string.Equals(tagUpper, TextBoxTag.PwdVerify, StringComparison.Ordinal))
                        {
                            tb.UseSystemPasswordChar = true;
                            AttachPasswordVerification(tb);
                        }

                        if (string.Equals(tagUpper, TextBoxTag.Price, StringComparison.Ordinal))
                        {
                            AttachPriceValidation(tb);
                        }
                    }
                }

                var cb = c as ComboBox;
                if (ConfigureCombosAsDropDownList && cb != null)
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;

                var nud = c as NumericUpDown;
                if (LockNumericUpDownTextBox && nud != null)
                {
                    var innerTb = nud.Controls.OfType<TextBox>().FirstOrDefault();
                    if (innerTb != null)
                    {
                        innerTb.ReadOnly = true;
                        innerTb.TabStop = false;
                    }
                }

                if (c.HasChildren)
                    ApplyPoliciesRecursive(c);
            }
        }

        private void AttachNumeric12Validation(TextBox tb)
        {
            tb.Validating -= Numeric12_Validating;
            tb.Validating += Numeric12_Validating;

            tb.TextChanged -= Numeric12_TextChanged;
            tb.TextChanged += Numeric12_TextChanged;
        }

        private void Numeric12_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNumeric(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_doc_validation_message"));
                e.Cancel = true;
            }
        }

        private void Numeric12_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNumeric(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private void AttachUrbansoftEmailValidation(TextBox tb)
        {
            tb.Validating -= UrbansoftEmail_Validating;
            tb.Validating += UrbansoftEmail_Validating;

            tb.TextChanged -= UrbansoftEmail_TextChanged;
            tb.TextChanged += UrbansoftEmail_TextChanged;
        }

        private void UrbansoftEmail_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidUrbansoftEmail(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_email_validation_message"));
                e.Cancel = true;
            }
        }

        private void UrbansoftEmail_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidUrbansoftEmail(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private void AttachSafeSqlValidation(TextBox tb)
        {
            tb.Validating -= SafeSql_Validating;
            tb.Validating += SafeSql_Validating;

            tb.TextChanged -= SafeSql_TextChanged;
            tb.TextChanged += SafeSql_TextChanged;
        }

        private void SafeSql_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsSafeForSql(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_safe_sql_validation_message"));
                e.Cancel = true;
            }
        }

        private void SafeSql_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsSafeForSql(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private void AttachPasswordVerification(TextBox tb)
        {
            tb.Validating -= PasswordVerification_Validating;
            tb.Validating += PasswordVerification_Validating;

            tb.TextChanged -= PasswordVerification_TextChanged;
            tb.TextChanged += PasswordVerification_TextChanged;
        }

        private void PasswordVerification_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNewPassword(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_password_validation_message"));
                e.Cancel = true;
            }
        }

        private void PasswordVerification_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNewPassword(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private void AttachPriceValidation(TextBox tb)
        {
            tb.Validating -= Price_Validating;
            tb.Validating += Price_Validating;

            tb.TextChanged -= Price_TextChanged;
            tb.TextChanged += Price_TextChanged;

            tb.Leave -= Price_Leave;
            tb.Leave += Price_Leave;
        }

        private void Price_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text == null ? string.Empty : tb.Text.Trim();

            if (txt.Length == 0 || InputSanitizer.IsValidPrice(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_price_validation_message"));
                e.Cancel = true;
            }
        }

        private void Price_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text == null ? string.Empty : tb.Text.Trim();
            if (txt.Length == 0 || InputSanitizer.IsValidPrice(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private void Price_Leave(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text == null ? string.Empty : tb.Text.Trim();
            if (txt.Length == 0) return;

            decimal _;
            if (InputSanitizer.TryParsePrice(txt, out _))
            {
                tb.Text = InputSanitizer.FormatPrice2(txt, CultureInfo.CurrentCulture, false);
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
        }
    }
}
