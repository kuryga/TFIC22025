using System;
using System.Windows.Forms;
using LoginBLL = BLL.Seguridad.LoginBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;
using PermisosBLL = BLL.Seguridad.PermisosBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class MainForm : BaseForm
    {
        public event Action LogoutRequested;
        private readonly PermisosBLL permisosBLL = PermisosBLL.GetInstance();

        private string closeSessionText = "";
        private string closeSessionTitle = "";

        public MainForm()
        {
            InitializeComponent();

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

            menuBackup.Click += (s, e) => AbrirFormulario(new BackupForm());
            menuRestore.Click += (s, e) => AbrirFormulario(new RestoreForm());

            menuCerrarSesion.Click += (s, e) =>
            {
                var resultado = MessageBox.Show(
                    closeSessionText,
                   closeSessionTitle,
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.OK)
                {
                    LoginBLL.GetInstance().Logout();
                    LogoutRequested?.Invoke();
                }
            };

            UpdateTexts();

            this.Shown += (s, e) => AplicarPermisos();
        }

        private void AplicarPermisos()
        {
            try
            {
                SetItemVisibility(menuUsuarios, permisosBLL.DebeVerUsuarios());
                SetItemVisibility(menuFamilias, permisosBLL.DebeVerFamilias());
                SetItemVisibility(menuPatentes, permisosBLL.DebeVerPatentes());
                SetItemVisibility(menuBitacora, permisosBLL.DebeVerBitacora());

                SetItemVisibility(menuMaquinaria, permisosBLL.DebeVerMaquinaria());
                SetItemVisibility(menuMateriales, permisosBLL.DebeVerMateriales());
                SetItemVisibility(menuServicios, permisosBLL.DebeVerServicios());
                SetItemVisibility(menuTipoEdif, permisosBLL.DebeVerTipoEdificacion());
                SetItemVisibility(menuMoneda, permisosBLL.DebeVerMoneda());

                SetItemVisibility(menuConsultar, permisosBLL.DebeVerCotizaciones());
                SetItemVisibility(menuNuevaCotizacion, permisosBLL.DebeCrearCotizacion());

                SetItemVisibility(menuBackup, permisosBLL.DebeVerBackup());
                SetItemVisibility(menuRestore, permisosBLL.DebeVerRestore());

                SetItemVisibility(gestionesMenu, HayAlgunoVisible(menuMaquinaria, menuMateriales, menuServicios, menuTipoEdif, menuMoneda));
                SetItemVisibility(usuariosMenu, HayAlgunoVisible(menuUsuarios, menuFamilias, menuPatentes));
                SetItemVisibility(cotizacionesMenu, HayAlgunoVisible(menuConsultar, menuNuevaCotizacion));
                SetItemVisibility(auditoriaMenu, HayAlgunoVisible(menuBitacora));
                SetItemVisibility(sistemaMenu, HayAlgunoVisible(menuBackup, menuRestore));

                menuStrip1.PerformLayout();
                menuStrip1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al aplicar permisos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetItemVisibility(ToolStripItem item, bool visible)
        {
            item.Available = visible;
            item.Visible = visible;
        }

        private bool HayAlgunoVisible(params ToolStripMenuItem[] items)
        {
            foreach (var i in items)
                if (i.Available || i.Visible) return true;
            return false;
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

            var desiredWidth = form.Width + panelContenedor.Margin.Horizontal + 13;
            var desiredHeight = form.Height + menuStrip1.Height + panelContenedor.Margin.Vertical + 35;

            this.Size = new System.Drawing.Size(desiredWidth, desiredHeight);
        }

        private void UpdateTexts()
        {
            cotizacionesMenu.Text = ParametrizacionBLL.GetInstance().GetLocalizable("quotes");
            menuConsultar.Text = ParametrizacionBLL.GetInstance().GetLocalizable("consult");
            menuNuevaCotizacion.Text = ParametrizacionBLL.GetInstance().GetLocalizable("new_quote");

            gestionesMenu.Text = ParametrizacionBLL.GetInstance().GetLocalizable("management");
            menuMaquinaria.Text = ParametrizacionBLL.GetInstance().GetLocalizable("machinery");
            menuMateriales.Text = ParametrizacionBLL.GetInstance().GetLocalizable("materials");
            menuServicios.Text = ParametrizacionBLL.GetInstance().GetLocalizable("services");
            menuTipoEdif.Text = ParametrizacionBLL.GetInstance().GetLocalizable("building_type");
            menuMoneda.Text = ParametrizacionBLL.GetInstance().GetLocalizable("currencies");

            usuariosMenu.Text = ParametrizacionBLL.GetInstance().GetLocalizable("users_permissions");
            menuUsuarios.Text = ParametrizacionBLL.GetInstance().GetLocalizable("users");
            menuFamilias.Text = ParametrizacionBLL.GetInstance().GetLocalizable("families");
            menuPatentes.Text = ParametrizacionBLL.GetInstance().GetLocalizable("patents");

            auditoriaMenu.Text = ParametrizacionBLL.GetInstance().GetLocalizable("audit");
            menuBitacora.Text = ParametrizacionBLL.GetInstance().GetLocalizable("log_view");

            sistemaMenu.Text = ParametrizacionBLL.GetInstance().GetLocalizable("system");
            menuBackup.Text = ParametrizacionBLL.GetInstance().GetLocalizable("backup");
            menuRestore.Text = ParametrizacionBLL.GetInstance().GetLocalizable("restore");

            menuCerrarSesion.Text = ParametrizacionBLL.GetInstance().GetLocalizable("logout");

            string menuText = ParametrizacionBLL.GetInstance().GetLocalizable("main_menu");
            string welcomeText = ParametrizacionBLL.GetInstance().GetLocalizable("welcome_message");
            string NombreEmpresa = ParametrizacionBLL.GetInstance().GetNombreEmpresa();
            string Username = UsuarioBLL.GetInstance().GetSesionActivaNombreCompleto();

            this.closeSessionText = ParametrizacionBLL.GetInstance().GetLocalizable("logout_confirm_question");
            this.closeSessionTitle = ParametrizacionBLL.GetInstance().GetLocalizable("logout_confirm_title");
            lblBienvenida.Text = $"{welcomeText} {Username}";

            this.Text = $"{menuText} - {NombreEmpresa}";
        }
    }
}
