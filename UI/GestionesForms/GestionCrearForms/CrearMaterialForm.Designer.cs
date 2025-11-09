namespace WinApp
{
    partial class CrearMaterialForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblUnidad = new System.Windows.Forms.Label();
            this.txtUnidad = new System.Windows.Forms.TextBox();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.lblUso = new System.Windows.Forms.Label();
            this.txtUso = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNombre
            // 
            this.lblNombre.Location = new System.Drawing.Point(12, 68);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(80, 23);
            this.lblNombre.TabIndex = 17;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(98, 65);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(220, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // lblUnidad
            // 
            this.lblUnidad.Location = new System.Drawing.Point(12, 98);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(80, 23);
            this.lblUnidad.TabIndex = 19;
            this.lblUnidad.Text = "Unidad:";
            // 
            // txtUnidad
            // 
            this.txtUnidad.Location = new System.Drawing.Point(98, 95);
            this.txtUnidad.Name = "txtUnidad";
            this.txtUnidad.Size = new System.Drawing.Size(220, 20);
            this.txtUnidad.TabIndex = 2;
            // 
            // lblPrecio
            // 
            this.lblPrecio.Location = new System.Drawing.Point(12, 128);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Size = new System.Drawing.Size(80, 23);
            this.lblPrecio.TabIndex = 21;
            this.lblPrecio.Text = "Precio:";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Location = new System.Drawing.Point(98, 125);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(220, 20);
            this.txtPrecio.TabIndex = 3;
            // 
            // lblUso
            // 
            this.lblUso.Location = new System.Drawing.Point(12, 158);
            this.lblUso.Name = "lblUso";
            this.lblUso.Size = new System.Drawing.Size(80, 23);
            this.lblUso.TabIndex = 23;
            this.lblUso.Text = "Uso por m²:";
            // 
            // txtUso
            // 
            this.txtUso.Location = new System.Drawing.Point(98, 155);
            this.txtUso.Name = "txtUso";
            this.txtUso.Size = new System.Drawing.Size(220, 20);
            this.txtUso.TabIndex = 4;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblTitle.Location = new System.Drawing.Point(12, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(306, 23);
            this.lblTitle.TabIndex = 29;
            this.lblTitle.Text = "Crear material";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(98, 190);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(134, 23);
            this.btnCrear.TabIndex = 5;
            this.btnCrear.Text = "Crear";
            // 
            // CrearMaterialForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 230);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblUso);
            this.Controls.Add(this.txtUso);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.txtUnidad);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Name = "CrearMaterialForm";
            this.Text = "Crear material";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblUnidad;
        private System.Windows.Forms.TextBox txtUnidad;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Label lblUso;
        private System.Windows.Forms.TextBox txtUso;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCrear;
    }
}
