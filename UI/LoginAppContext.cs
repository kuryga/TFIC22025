using System;
using System.Windows.Forms;

namespace UI
{
    // notas mentales:
    // se aplica mediator, loginapp context actua como mediador y evita que los forms se conozcan
    // tambien usa observers, loginappcontext es un observador
    // y el loginform/mainform son los observables LoginSucceeded y LogoutRequested respectivamente
    // 
    public sealed class LoginAppContext : ApplicationContext
    {
        private Form _active;

        public LoginAppContext()
        {
            SwitchTo(new LoginForm());
        }

        private void SwitchTo(Form next)
        {
            var prev = _active;

            if (next is LoginForm lf)
                lf.LoginSucceeded += OnLoginSucceeded;
            if (next is MainForm mf)
                mf.LogoutRequested += OnLogoutRequested;

            next.FormClosed += OnActiveFormClosed;

            _active = next;
            MainForm = next;
            next.Show();

            if (prev != null)
            {
                if (prev is LoginForm plf)
                    plf.LoginSucceeded -= OnLoginSucceeded;
                if (prev is MainForm pmf)
                    pmf.LogoutRequested -= OnLogoutRequested;

                prev.FormClosed -= OnActiveFormClosed;
                prev.Close();
                prev.Dispose();
            }
        }

        private void OnLoginSucceeded()
        {
            SwitchTo(new MainForm());
        }

        private void OnLogoutRequested()
        {
            SwitchTo(new LoginForm());
        }

        private void OnActiveFormClosed(object sender, FormClosedEventArgs e)
        {
            if (ReferenceEquals(sender, _active))
                ExitThread();
        }
    }
}