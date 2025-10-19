using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BLL.Seguridad;

namespace UI
{
    public partial class GestionarPatentesForm : Form
    {
        private int _usuarioSeleccionadoId = 0;
        private HashSet<int> _patentesAsignadasOriginal = new HashSet<int>();

        public GestionarPatentesForm()
        {
            InitializeComponent();
            ConfigurarGrids();
            CargarUsuarios();
        }

        private void ConfigurarGrids()
        {
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AllowUserToAddRows = false;

            dgvDisponibles.MultiSelect = false;
            dgvDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDisponibles.AllowUserToAddRows = false;

            dgvAsignadas.MultiSelect = false;
            dgvAsignadas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignadas.AllowUserToAddRows = false;

            // Usuarios
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.Columns.Clear();
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                Name = "IdUsuario",
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nombre completo",
                Name = "NombreCompleto",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Email",
                Name = "CorreoElectronico",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            // Patentes disponibles
            dgvDisponibles.AutoGenerateColumns = false;
            dgvDisponibles.Columns.Clear();
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                Name = "IdPatente",
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Patente",
                Name = "NombrePatente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Descripción",
                Name = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvAsignadas.AutoGenerateColumns = false;
            dgvAsignadas.Columns.Clear();
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                Name = "IdPatente",
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Patente",
                Name = "NombrePatente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Descripción",
                Name = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
        }

        private void CargarUsuarios()
        {
            dgvUsuarios.Rows.Clear();

            var usuarios = UsuarioBLL.GetInstance().GetAll();

            foreach (var u in usuarios)
            {
                var nombreCompleto = $"{u.NombreUsuario} {u.ApellidoUsuario}".Trim();
                dgvUsuarios.Rows.Add(u.IdUsuario.ToString(), nombreCompleto, u.CorreoElectronico);
            }

            if (dgvUsuarios.Rows.Count > 0)
            {
                dgvUsuarios.ClearSelection();
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.IsNewRow) return;

            if (!int.TryParse(Convert.ToString(dgvUsuarios.CurrentRow.Cells["IdUsuario"].Value), out var idUsuario))
                return;

            _usuarioSeleccionadoId = idUsuario;

            try
            {
                CargarPatentesParaUsuario(_usuarioSeleccionadoId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando patentes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPatentesParaUsuario(int idUsuario)
        {
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();
            _patentesAsignadasOriginal.Clear();

            var todas = PermisosBLL.GetInstance().GetAllPatentes();
            var asignadas = PermisosBLL.GetInstance().GetPatentesByUsuario(idUsuario) ?? new List<BE.Patente>();

            foreach (var p in asignadas)
                _patentesAsignadasOriginal.Add(p.IdPatente);

            var setAsignadas = new HashSet<int>(asignadas.Select(p => p.IdPatente));

            foreach (var p in todas)
            {
                if (setAsignadas.Contains(p.IdPatente))
                    dgvAsignadas.Rows.Add(p.IdPatente, p.NombrePatente, p.Descripcion);
                else
                    dgvDisponibles.Rows.Add(p.IdPatente, p.NombrePatente, p.Descripcion);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDisponibles.CurrentRow == null) return;

            var id = Convert.ToInt32(dgvDisponibles.CurrentRow.Cells["IdPatente"].Value);
            var nombre = Convert.ToString(dgvDisponibles.CurrentRow.Cells["NombrePatente"].Value);
            var desc = Convert.ToString(dgvDisponibles.CurrentRow.Cells["Descripcion"].Value);

            dgvDisponibles.Rows.RemoveAt(dgvDisponibles.CurrentRow.Index);
            dgvAsignadas.Rows.Add(id, nombre, desc);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAsignadas.CurrentRow == null) return;

            var id = Convert.ToInt32(dgvAsignadas.CurrentRow.Cells["IdPatente"].Value);
            var nombre = Convert.ToString(dgvAsignadas.CurrentRow.Cells["NombrePatente"].Value);
            var desc = Convert.ToString(dgvAsignadas.CurrentRow.Cells["Descripcion"].Value);

            dgvAsignadas.Rows.RemoveAt(dgvAsignadas.CurrentRow.Index);
            dgvDisponibles.Rows.Add(id, nombre, desc);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionadoId <= 0)
            {
                MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var asignadasAhora = new HashSet<int>();
            foreach (DataGridViewRow r in dgvAsignadas.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.Cells["IdPatente"].Value == null) continue;
                asignadasAhora.Add(Convert.ToInt32(r.Cells["IdPatente"].Value));
            }

            var hayCambios = !_patentesAsignadasOriginal.SetEquals(asignadasAhora);

            if (!hayCambios)
            {
                MessageBox.Show("No hay cambios para guardar.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                PermisosBLL.GetInstance().SetPatentesForUsuario(_usuarioSeleccionadoId, asignadasAhora);
                _patentesAsignadasOriginal = new HashSet<int>(asignadasAhora);
                MessageBox.Show("Patentes asignadas guardadas correctamente.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
