using System.Windows.Forms;

namespace UI
{
    partial class MainForm
    {
        private MenuStrip menuStrip1;
        private Panel panelContenedor;
        private Label lblBienvenida;

        private ToolStripMenuItem gestionesMenu;
        private ToolStripMenuItem usuariosMenu;
        private ToolStripMenuItem cotizacionesMenu;
        private ToolStripMenuItem seguridadMenu;
        private ToolStripMenuItem menuCerrarSesion;

        private ToolStripMenuItem menuMaquinaria, menuMateriales, menuMoneda,
                                  menuServicios, menuPerfiles, menuItems, menuTipoEdif,
                                  menuUsuarios, menuFamilias, menuPatentes, menuConsultar,
                                  menuBitacora, menuNuevaCotizacion;

        private void InitializeComponent()
        {
            this.menuStrip1 = new MenuStrip();
            this.panelContenedor = new Panel();
            this.lblBienvenida = new Label();

            // Menú principal
            this.gestionesMenu = new ToolStripMenuItem("Gestiones");
            this.usuariosMenu = new ToolStripMenuItem("Usuarios y Permisos");
            this.cotizacionesMenu = new ToolStripMenuItem("Cotizaciones");
            this.seguridadMenu = new ToolStripMenuItem("Seguridad");
            this.menuCerrarSesion = new ToolStripMenuItem("Cerrar sesión");

            // Submenús
            this.menuMaquinaria = new ToolStripMenuItem("Maquinaria");
            this.menuMateriales = new ToolStripMenuItem("Materiales");
            this.menuMoneda = new ToolStripMenuItem("Moneda");
            this.menuServicios = new ToolStripMenuItem("Servicios");
            // this.menuPerfiles = new ToolStripMenuItem("Perfiles Profesionales");
            // this.menuItems = new ToolStripMenuItem("Ítems Personalizados");
            this.menuTipoEdif = new ToolStripMenuItem("Tipo Edificación");

            this.menuUsuarios = new ToolStripMenuItem("Usuarios");
            this.menuFamilias = new ToolStripMenuItem("Familias");
            this.menuPatentes = new ToolStripMenuItem("Patentes");

            this.menuConsultar = new ToolStripMenuItem("Consultar");
            this.menuNuevaCotizacion = new ToolStripMenuItem("Nueva cotizacion");

            this.menuBitacora = new ToolStripMenuItem("Consultar Bitacora");

            // Estructura de menús
            this.gestionesMenu.DropDownItems.AddRange(new ToolStripItem[] {
                menuMaquinaria, menuMateriales, menuMoneda,
                menuServicios, menuTipoEdif
            });

            this.usuariosMenu.DropDownItems.AddRange(new ToolStripItem[] {
                menuUsuarios, menuFamilias, menuPatentes
            });

            this.cotizacionesMenu.DropDownItems.AddRange(new ToolStripItem[] {
                menuConsultar, menuNuevaCotizacion
            });

            this.seguridadMenu.DropDownItems.Add(menuBitacora);


            this.menuStrip1.Items.AddRange(new ToolStripItem[] {
                cotizacionesMenu, gestionesMenu, usuariosMenu, seguridadMenu, menuCerrarSesion
            });

            this.menuStrip1.Dock = DockStyle.Top;

            // Panel contenedor
            this.panelContenedor.Dock = DockStyle.Fill;
            this.panelContenedor.BorderStyle = BorderStyle.FixedSingle;

            // Label bienvenida
            this.lblBienvenida.Text = "Bienvenido al sistema ''Usuario logueado''";
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            this.lblBienvenida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBienvenida.Location = new System.Drawing.Point(100, 250); // Ajustable
            this.lblBienvenida.Anchor = AnchorStyles.None;

            // Agregar al contenedor
            this.panelContenedor.Controls.Add(this.lblBienvenida);

            // Formulario principal
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Nombre parametrizacion Empresa S.A";
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
