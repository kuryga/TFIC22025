﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;
using Usuario = BE.Usuario;

namespace UI
{
    public partial class GestionarUsuarioForm : BaseForm
    {
        private Usuario usr = null;

        public GestionarUsuarioForm() : this(null) { }

        public GestionarUsuarioForm(Usuario UsuarioAEditar)
        {
            usr = UsuarioAEditar;
            InitializeComponent();
            UpdateTexts();

            btnCrear.Click -= btnCrear_Click;
            btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {
            txtTelefono.Tag = "AR_PHONE";
            txtDocumento.Tag = "NUM_12";
            txtCorreo.Tag = "MAIL_URBANSOFT";
            txtDireccion.Tag = "SAFE";
            txtNombre.Tag = "SAFE";
            txtApellido.Tag = "SAFE";
 
            lblNombre.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_firstname_label");
            lblApellido.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_lastname_label");
            lblDocumento.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_document_label");
            lblCorreo.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_email_label");
            lblTelefono.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_phone_label");
            lblDireccion.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_address_label");

            btnCrear.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_create_button");

            string titleText = ParametrizacionBLL.GetInstance().GetLocalizable("user_create_title");
            string NombreEmpresa = ParametrizacionBLL.GetInstance().GetNombreEmpresa();

            lblTitle.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_create_title");
            this.Text = $"{titleText} - {NombreEmpresa}";
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text?.Trim() ?? "";
                string apellido = txtApellido.Text?.Trim() ?? "";
                string correo = txtCorreo.Text?.Trim() ?? "";
                string telefono = txtTelefono.Text?.Trim() ?? "";
                string direccion = txtDireccion.Text?.Trim() ?? "";
                string documento = txtDocumento.Text?.Trim() ?? "";

                var faltantes = new List<string>();
                if (string.IsNullOrWhiteSpace(nombre)) faltantes.Add("Nombre");
                if (string.IsNullOrWhiteSpace(apellido)) faltantes.Add("Apellido");
                if (string.IsNullOrWhiteSpace(correo)) faltantes.Add("Correo");
                if (string.IsNullOrWhiteSpace(telefono)) faltantes.Add("Teléfono");
                if (string.IsNullOrWhiteSpace(direccion)) faltantes.Add("Dirección");

                if (faltantes.Count > 0)
                {
                    MessageBox.Show(
                         ParametrizacionBLL.GetInstance().GetLocalizable("user_required_fields_message") + string.Join(", ", faltantes),
                         ParametrizacionBLL.GetInstance().GetLocalizable("user_required_fields_title"),
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Warning
                    );
                    return;
                }

                var nuevo = new Usuario
                {
                    IdUsuario = 0,
                    NombreUsuario = nombre,
                    ApellidoUsuario = apellido,
                    CorreoElectronico = correo,
                    TelefonoContacto = telefono,
                    DireccionUsuario = direccion,
                    NumeroDocumento = documento
                };

                UsuarioBLL.GetInstance().Create(nuevo);

                MessageBox.Show(
                    ParametrizacionBLL.GetInstance().GetLocalizable("user_created_success"),
                    "OK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    msg += Environment.NewLine + ex.InnerException.Message;

                MessageBox.Show(
                    ParametrizacionBLL.GetInstance().GetLocalizable("user_create_error_message") + msg,
                    ParametrizacionBLL.GetInstance().GetLocalizable("user_create_error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
