using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

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
                if (EnableSanitization && c is TextBox tbSan)
                {
                    if (!(tbSan.Tag is string tagSan && tagSan.Equals("NoSanitize", StringComparison.OrdinalIgnoreCase)))
                        InputSanitizer.ProtectTextBox(tbSan);
                }

                if (EnableArPhoneValidation && c is TextBox tbPhone)
                {
                    if (tbPhone.Tag is string tag && tag.Equals("AR_PHONE", StringComparison.OrdinalIgnoreCase))
                        PhoneValidator.Attach(tbPhone, _sharedErrorProvider);
                }

                if (c is TextBox tb)
                {
                    var tag = tb.Tag as string;

                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        if (tag.Equals("NUM_12", StringComparison.OrdinalIgnoreCase))
                            AttachNumeric12Validation(tb);

                        if (tag.Equals("MAIL_URBANSOFT", StringComparison.OrdinalIgnoreCase))
                            AttachUrbansoftEmailValidation(tb);
                    }
                }

                if (ConfigureCombosAsDropDownList && c is ComboBox cb)
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;

                if (LockNumericUpDownTextBox && c is NumericUpDown nud)
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
            if (sender is TextBox tb)
            {
                var txt = tb.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNumeric(txt))
                {
                    _sharedErrorProvider.SetError(tb, string.Empty);
                }
                else
                {
                    _sharedErrorProvider.SetError(tb, "Solo números, máximo 12 caracteres");
                    e.Cancel = true;
                }
            }
        }

        private void Numeric12_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                var txt = tb.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidNumeric(txt))
                    _sharedErrorProvider.SetError(tb, string.Empty);
            }
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
            if (sender is TextBox tb)
            {
                var txt = tb.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidUrbansoftEmail(txt))
                {
                    _sharedErrorProvider.SetError(tb, string.Empty);
                }
                else
                {
                    _sharedErrorProvider.SetError(tb, "Correo inválido. Debe terminar en @urbansoft.com");
                    e.Cancel = true;
                }
            }
        }

        private void UrbansoftEmail_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                var txt = tb.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(txt) || InputSanitizer.IsValidUrbansoftEmail(txt))
                    _sharedErrorProvider.SetError(tb, string.Empty);
            }
        }
    }
}
