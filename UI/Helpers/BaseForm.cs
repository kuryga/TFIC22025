using System;
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
    }
}
