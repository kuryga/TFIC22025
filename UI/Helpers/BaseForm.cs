using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
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

                    if (string.Equals(tagUpper, "PASSWORD", StringComparison.Ordinal))
                    {
                        tb.UseSystemPasswordChar = true;
                        AttachPasswordValidation(tb);
                    }
                    else
                    {
                        if (EnableSanitization)
                        {
                            if (!(tb.Tag is string tagSan && tagSan.Equals("NoSanitize", StringComparison.OrdinalIgnoreCase)))
                                InputSanitizer.ProtectTextBox(tb);
                        }

                        if (!string.IsNullOrEmpty(tagUpper))
                        {
                            if (EnableArPhoneValidation && string.Equals(tagUpper, "AR_PHONE", StringComparison.Ordinal))
                                PhoneValidator.Attach(tb, _sharedErrorProvider);

                            if (string.Equals(tagUpper, "NUM_12", StringComparison.Ordinal))
                                AttachNumeric12Validation(tb);

                            if (string.Equals(tagUpper, "MAIL_URBANSOFT", StringComparison.Ordinal))
                                AttachUrbansoftEmailValidation(tb);

                            if (string.Equals(tagUpper, "SAFE", StringComparison.Ordinal))
                                AttachSafeSqlValidation(tb);
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

        private void AttachPasswordValidation(TextBox tb)
        {
            tb.Validating -= Password_Validating;
            tb.Validating += Password_Validating;

            tb.TextChanged -= Password_TextChanged;
            tb.TextChanged += Password_TextChanged;
        }

        private void Password_Validating(object sender, CancelEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text ?? string.Empty;

            if (string.IsNullOrEmpty(txt) || IsStrongPassword(txt))
            {
                _sharedErrorProvider.SetError(tb, string.Empty);
            }
            else
            {
                _sharedErrorProvider.SetError(tb, ParametrizacionBLL.GetInstance().GetLocalizable("user_password_validation_message"));
                e.Cancel = true;
            }
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            var txt = tb.Text ?? string.Empty;
            if (string.IsNullOrEmpty(txt) || IsStrongPassword(txt))
                _sharedErrorProvider.SetError(tb, string.Empty);
        }

        private static bool IsStrongPassword(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            var rx = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$");
            return rx.IsMatch(input);
        }
    }
}
