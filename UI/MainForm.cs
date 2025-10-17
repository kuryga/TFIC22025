using System;
using System.Windows.Forms;
using LoginBLL = BLL.Seguridad.LoginBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using Parametrizacion = BE.Params.Parametrizacion;

namespace UI
{
    public partial class MainForm : Form
    {
        public event Action LogoutRequested;

        public MainForm()
        {
            InitializeComponent();

            // Asignar eventos a los ítems del menú
            menuMaquinaria.Click += (s, e) => AbrirFormulario(new GestionarMaquinariaForm());
            menuMateriales.Click += (s, e) => AbrirFormulario(new GestionarMaterialForm());
            menuMoneda.Click += (s, e) => AbrirFormulario(new GestionarMonedaForm());
            menuServicios.Click += (s, e) => AbrirFormulario(new GestionarServicioAdicionalForm());
            menuBitacora.Click += (s, e) => AbrirFormulario(new ConsultarBitacoraForm());
            menuTipoEdif.Click += (s, e) => AbrirFormulario(new GestionarTipoEdificacionForm());

            menuUsuarios.Click += (s, e) => AbrirFormulario(new GestionarUsuariosForm());
            menuFamilias.Click += (s, e) => AbrirFormulario(new GestionarFamiliasForm());
            menuPatentes.Click += (s, e) => AbrirFormulario(new GestionarPatentesForm());

            menuConsultar.Click += (s, e) => AbrirFormulario(new ConsultarCotizacionesForm());
            menuNuevaCotizacion.Click += (s, e) => AbrirFormulario(new NuevaCotizacionForm());
            // Cerrar sesión
            menuCerrarSesion.Click += (s, e) =>
            {
                var resultado = MessageBox.Show(
                    "¿Seguro que querés cerrar sesión?",
                    "Confirmar cierre de sesión",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.OK)
                {
                    LoginBLL.GetInstance().Logout();
                    LogoutRequested?.Invoke();
                }
            };

            string Username = UsuarioBLL.GetInstance().GetSesionActivaNombreCompleto();
            lblBienvenida.Text = $"Bienvenido al sistema {Username}";

            Parametrizacion param = ParametrizacionBLL.GetInstance().GetParametrizacion();

            this.Text = $"Menu principal - {param.NombreEmpresa}";
        }

        private void AbrirFormulario(Form form)
        {
            if (lblBienvenida.Visible)
                lblBienvenida.Visible = false;

            panelContenedor.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.None;

            panelContenedor.Controls.Add(form);
            form.Show();
            form.PerformLayout();

            var desiredWidth = form.Width + panelContenedor.Margin.Horizontal + 40;
            var desiredHeight = form.Height + menuStrip1.Height + panelContenedor.Margin.Vertical + 60;

            this.Size = new System.Drawing.Size(desiredWidth, desiredHeight);
        }
    }
}
