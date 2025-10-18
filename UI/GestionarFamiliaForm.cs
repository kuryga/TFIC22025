using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BLL.Seguridad;

namespace UI
{
    public partial class GestionarFamiliaForm : Form
    {
        private readonly bool _isEdit;
        private readonly int _familiaId;
        private HashSet<int> _patentesAsignadasOriginal = new HashSet<int>();

        public GestionarFamiliaForm() : this(null) { }

        public GestionarFamiliaForm(BE.Familia familiaAEditar)
        {
            InitializeComponent();
            ConfigurarControles();

            _isEdit = familiaAEditar != null;
            _familiaId = familiaAEditar?.IdFamilia ?? 0;

            if (_isEdit)
            {
                txtId.Text = familiaAEditar.IdFamilia.ToString();
                txtNombre.Text = familiaAEditar.NombreFamilia ?? string.Empty;
                txtDesc.Text = familiaAEditar.Descripcion ?? string.Empty;
                txtId.Enabled = false;

                CargarPatentesParaFamilia(_familiaId);
            }
            else
            {
                txtId.Text = string.Empty;
                txtNombre.Text = string.Empty;
                txtDesc.Text = string.Empty;
                txtId.Enabled = false;

                CargarPatentesParaFamilia(0);
            }
        }

        private void ConfigurarControles()
        {
            txtId.ReadOnly = true;
            txtId.TabStop = false;

            ConfigurarGridPatentes(dgvDisponibles);
            ConfigurarGridPatentes(dgvAsignadas);

            dgvDisponibles.MultiSelect = false;
            dgvDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDisponibles.AllowUserToAddRows = false;

            dgvAsignadas.MultiSelect = false;
            dgvAsignadas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignadas.AllowUserToAddRows = false;

        }

        private static void ConfigurarGridPatentes(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id",
                Name = "IdPatente",
                Width = 50
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Patente",
                Name = "NombrePatente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Descripción",
                Name = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private void CargarPatentesParaFamilia(int idFamilia)
        {
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();
            _patentesAsignadasOriginal.Clear();

            var todas = PermisosBLL.GetInstance().GetAllPatentes() ?? new List<BE.Patente>();
            var asignadas = (idFamilia > 0)
                ? (PermisosBLL.GetInstance().GetPatentesByFamilia(idFamilia) ?? new List<BE.Patente>())
                : new List<BE.Patente>();

            // guardar set original para detectar cambios si hace falta luego
            foreach (var p in asignadas) _patentesAsignadasOriginal.Add(p.IdPatente);

            var setAsignadas = new HashSet<int>(asignadas.Select(p => p.IdPatente));

            foreach (var p in todas)
            {
                var row = new object[] { p.IdPatente, p.NombrePatente, p.Descripcion };
                if (setAsignadas.Contains(p.IdPatente))
                    dgvAsignadas.Rows.Add(row);
                else
                    dgvDisponibles.Rows.Add(row);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDisponibles.CurrentRow == null || dgvDisponibles.CurrentRow.IsNewRow) return;

            var id = Convert.ToInt32(dgvDisponibles.CurrentRow.Cells["IdPatente"].Value);
            var nom = Convert.ToString(dgvDisponibles.CurrentRow.Cells["NombrePatente"].Value);
            var des = Convert.ToString(dgvDisponibles.CurrentRow.Cells["Descripcion"].Value);

            int idx = dgvDisponibles.CurrentRow.Index;
            dgvDisponibles.Rows.RemoveAt(idx);

            dgvAsignadas.ClearSelection();
            var newIdx = dgvAsignadas.Rows.Add(id, nom, des);
            dgvAsignadas.Rows[newIdx].Selected = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAsignadas.CurrentRow == null || dgvAsignadas.CurrentRow.IsNewRow) return;

            var id = Convert.ToInt32(dgvAsignadas.CurrentRow.Cells["IdPatente"].Value);
            var nom = Convert.ToString(dgvAsignadas.CurrentRow.Cells["NombrePatente"].Value);
            var des = Convert.ToString(dgvAsignadas.CurrentRow.Cells["Descripcion"].Value);

            int idx = dgvAsignadas.CurrentRow.Index;
            dgvAsignadas.Rows.RemoveAt(idx);

            dgvDisponibles.ClearSelection();
            var newIdx = dgvDisponibles.Rows.Add(id, nom, des);
            dgvDisponibles.Rows[newIdx].Selected = true;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {

            var nombre = (txtNombre.Text ?? string.Empty).Trim();
            var descripcion = (txtDesc.Text ?? string.Empty).Trim();

            var patentesAhora = new HashSet<int>();
            foreach (DataGridViewRow r in dgvAsignadas.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.Cells["IdPatente"].Value == null) continue;
                patentesAhora.Add(Convert.ToInt32(r.Cells["IdPatente"].Value));
            }

            if (_isEdit)
            {
                // EDITAR familia existente
                string resumen = $"Editar Familia Id={_familiaId}\nNombre=\"{nombre}\"\nDesc=\"{descripcion}\"\nPatentes: [{string.Join(", ", patentesAhora)}]";
                MessageBox.Show(resumen, "Editar (simulado)", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: luego:
                // 1) Validar datos
                // 2) Llamar a BLL/DAL para actualizar Familia (nombre/descripcion)
                // 3) Llamar a DAL para actualizar relaciones FamiliaPatente con 'patentesAhora'
            }
            else
            {
                // CREAR nueva familia
                string resumen = $"Crear Familia\nNombre=\"{nombre}\"\nDesc=\"{descripcion}\"\nPatentes: [{string.Join(", ", patentesAhora)}]";
                MessageBox.Show(resumen, "Crear (simulado)", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: luego:
                // 1) Crear familia y obtener nuevo Id
                // 2) Insertar relaciones en FamiliaPatente según 'patentesAhora'
                // 3) Refrescar DVH/DVV con tu DalToolkit
            }
        }
    }
}
