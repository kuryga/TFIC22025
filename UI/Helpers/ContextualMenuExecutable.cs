using System;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI.Infrastructure
{
    public interface IContextualMenuExecutable
    {
        void Configure(string title, string text);
        bool HandleKey(Keys keyData);
        void ShowHelp();
    }

    public sealed class ContextualMenuExecutable : IContextualMenuExecutable
    {
        private readonly Form _owner;

        // valores por defecto vienen de Parametrizacion
        private string _title;
        private string _text;

        private static string L(string code)
        {
            return ParametrizacionBLL.GetInstance().GetLocalizable(code) ?? string.Empty;
        }

        public ContextualMenuExecutable(Form owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            _owner = owner;

            // Defaults localizados
            _title = L("context_help_assistant_title");
            _text = L("context_help_no_info_message");
            if (string.IsNullOrWhiteSpace(_title)) _title = "Asistente de ayuda";
            if (string.IsNullOrWhiteSpace(_text)) _text = "Sin información disponible.";
        }

        public void Configure(string title, string text)
        {
            if (!string.IsNullOrWhiteSpace(title)) _title = title;
            if (!string.IsNullOrWhiteSpace(text)) _text = text;
        }

        public bool HandleKey(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                ShowHelp();
                return true;
            }
            return false;
        }

        public void ShowHelp()
        {
            try
            {
                MessageBox.Show(
                    _owner,
                    _text,
                    _title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch
            {
                var errTitle = L("context_help_error_title");
                var errMsg = L("context_help_error_message");

                if (string.IsNullOrWhiteSpace(errTitle)) errTitle = "Error de ayuda";
                if (string.IsNullOrWhiteSpace(errMsg))
                    errMsg = "Ocurrió un error inesperado al intentar mostrar la ayuda. " +
                             "Por favor, comuníquese con el soporte técnico de la empresa para obtener asistencia.";

                MessageBox.Show(
                    _owner,
                    errMsg,
                    errTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
