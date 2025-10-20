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
        private ToolStripMenuItem auditoriaMenu;
        private ToolStripMenuItem menuCerrarSesion;

        private ToolStripMenuItem menuMaquinaria, menuMateriales, menuMoneda,
                                  menuServicios, menuPerfiles, menuItems, menuTipoEdif,
                                  menuUsuarios, menuFamilias, menuPatentes, menuConsultar,
                                  menuBitacora, menuNuevaCotizacion;

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cotizacionesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuConsultar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNuevaCotizacion = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMaquinaria = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMateriales = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoneda = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServicios = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTipoEdif = new System.Windows.Forms.ToolStripMenuItem();
            this.usuariosMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFamilias = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPatentes = new System.Windows.Forms.ToolStripMenuItem();
            this.auditoriaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBitacora = new System.Windows.Forms.ToolStripMenuItem();
            this.sistemaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCerrarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panelContenedor.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cotizacionesMenu,
            this.gestionesMenu,
            this.usuariosMenu,
            this.auditoriaMenu,
            this.sistemaMenu,
            this.menuCerrarSesion});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1000, 24);
            this.menuStrip1.TabIndex = 1;
            // 
            // cotizacionesMenu
            // 
            this.cotizacionesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuConsultar,
            this.menuNuevaCotizacion});
            this.cotizacionesMenu.Name = "cotizacionesMenu";
            this.cotizacionesMenu.Size = new System.Drawing.Size(86, 20);
            this.cotizacionesMenu.Text = "Cotizaciones";
            // 
            // menuConsultar
            // 
            this.menuConsultar.Name = "menuConsultar";
            this.menuConsultar.Size = new System.Drawing.Size(165, 22);
            this.menuConsultar.Text = "Consultar";
            // 
            // menuNuevaCotizacion
            // 
            this.menuNuevaCotizacion.Name = "menuNuevaCotizacion";
            this.menuNuevaCotizacion.Size = new System.Drawing.Size(165, 22);
            this.menuNuevaCotizacion.Text = "Nueva cotizacion";
            // 
            // gestionesMenu
            // 
            this.gestionesMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMaquinaria,
            this.menuMateriales,
            this.menuMoneda,
            this.menuServicios,
            this.menuTipoEdif});
            this.gestionesMenu.Name = "gestionesMenu";
            this.gestionesMenu.Size = new System.Drawing.Size(70, 20);
            this.gestionesMenu.Text = "Gestiones";
            // 
            // menuMaquinaria
            // 
            this.menuMaquinaria.Name = "menuMaquinaria";
            this.menuMaquinaria.Size = new System.Drawing.Size(158, 22);
            this.menuMaquinaria.Text = "Maquinaria";
            // 
            // menuMateriales
            // 
            this.menuMateriales.Name = "menuMateriales";
            this.menuMateriales.Size = new System.Drawing.Size(158, 22);
            this.menuMateriales.Text = "Materiales";
            // 
            // menuMoneda
            // 
            this.menuMoneda.Name = "menuMoneda";
            this.menuMoneda.Size = new System.Drawing.Size(158, 22);
            this.menuMoneda.Text = "Moneda";
            // 
            // menuServicios
            // 
            this.menuServicios.Name = "menuServicios";
            this.menuServicios.Size = new System.Drawing.Size(158, 22);
            this.menuServicios.Text = "Servicios";
            // 
            // menuTipoEdif
            // 
            this.menuTipoEdif.Name = "menuTipoEdif";
            this.menuTipoEdif.Size = new System.Drawing.Size(158, 22);
            this.menuTipoEdif.Text = "Tipo Edificación";
            // 
            // usuariosMenu
            // 
            this.usuariosMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUsuarios,
            this.menuFamilias,
            this.menuPatentes});
            this.usuariosMenu.Name = "usuariosMenu";
            this.usuariosMenu.Size = new System.Drawing.Size(124, 20);
            this.usuariosMenu.Text = "Usuarios y Permisos";
            // 
            // menuUsuarios
            // 
            this.menuUsuarios.Name = "menuUsuarios";
            this.menuUsuarios.Size = new System.Drawing.Size(119, 22);
            this.menuUsuarios.Text = "Usuarios";
            // 
            // menuFamilias
            // 
            this.menuFamilias.Name = "menuFamilias";
            this.menuFamilias.Size = new System.Drawing.Size(119, 22);
            this.menuFamilias.Text = "Familias";
            // 
            // menuPatentes
            // 
            this.menuPatentes.Name = "menuPatentes";
            this.menuPatentes.Size = new System.Drawing.Size(119, 22);
            this.menuPatentes.Text = "Patentes";
            // 
            // auditoriaMenu
            // 
            this.auditoriaMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBitacora});
            this.auditoriaMenu.Name = "auditoriaMenu";
            this.auditoriaMenu.Size = new System.Drawing.Size(68, 20);
            this.auditoriaMenu.Text = "Auditoria";
            // 
            // menuBitacora
            // 
            this.menuBitacora.Name = "menuBitacora";
            this.menuBitacora.Size = new System.Drawing.Size(180, 22);
            this.menuBitacora.Text = "Consultar Bitacora";
            // 
            // sistemaMenu
            // 
            this.sistemaMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBackup,
            this.menuRestore});
            this.sistemaMenu.Name = "sistemaMenu";
            this.sistemaMenu.Size = new System.Drawing.Size(60, 20);
            this.sistemaMenu.Text = "Sistema";
            // 
            // menuBackup
            // 
            this.menuBackup.Name = "menuBackup";
            this.menuBackup.Size = new System.Drawing.Size(113, 22);
            this.menuBackup.Text = "Backup";
            // 
            // menuRestore
            // 
            this.menuRestore.Name = "menuRestore";
            this.menuRestore.Size = new System.Drawing.Size(113, 22);
            this.menuRestore.Text = "Restore";
            // 
            // menuCerrarSesion
            // 
            this.menuCerrarSesion.Name = "menuCerrarSesion";
            this.menuCerrarSesion.Size = new System.Drawing.Size(87, 20);
            this.menuCerrarSesion.Text = "Cerrar sesión";
            // 
            // panelContenedor
            // 
            this.panelContenedor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelContenedor.Controls.Add(this.lblBienvenida);
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(0, 24);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1000, 676);
            this.panelContenedor.TabIndex = 0;
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBienvenida.Location = new System.Drawing.Point(499, 537);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(388, 25);
            this.lblBienvenida.TabIndex = 0;
            this.lblBienvenida.Text = "Bienvenido al sistema \'\'Usuario logueado\'\'";
            this.lblBienvenida.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nombre parametrizacion Empresa S.A";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelContenedor.ResumeLayout(false);
            this.panelContenedor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ToolStripMenuItem sistemaMenu;
        private ToolStripMenuItem menuBackup;
        private ToolStripMenuItem menuRestore;
    }
}
