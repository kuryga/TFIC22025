namespace WinApp
{
    partial class CrearMonedaForm
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
            this.lblSimbolo = new System.Windows.Forms.Label();
            this.txtSimbolo = new System.Windows.Forms.TextBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
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
            // lblSimbolo
            // 
            this.lblSimbolo.Location = new System.Drawing.Point(12, 98);
            this.lblSimbolo.Name = "lblSimbolo";
            this.lblSimbolo.Size = new System.Drawing.Size(80, 23);
            this.lblSimbolo.TabIndex = 19;
            this.lblSimbolo.Text = "Símbolo:";
            // 
            // txtSimbolo
            // 
            this.txtSimbolo.Location = new System.Drawing.Point(98, 95);
            this.txtSimbolo.Name = "txtSimbolo";
            this.txtSimbolo.Size = new System.Drawing.Size(220, 20);
            this.txtSimbolo.TabIndex = 2;
            // 
            // lblValor
            // 
            this.lblValor.Location = new System.Drawing.Point(12, 128);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(80, 23);
            this.lblValor.TabIndex = 21;
            this.lblValor.Text = "Tasa:";
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(98, 125);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(220, 20);
            this.txtValor.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblTitle.Location = new System.Drawing.Point(12, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(306, 23);
            this.lblTitle.TabIndex = 29;
            this.lblTitle.Text = "Crear moneda";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(98, 165);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(134, 23);
            this.btnCrear.TabIndex = 4;
            this.btnCrear.Text = "Crear";
            // 
            // CrearMonedaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 205);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.lblSimbolo);
            this.Controls.Add(this.txtSimbolo);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Name = "CrearMonedaForm";
            this.Text = "Crear moneda";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblSimbolo;
        private System.Windows.Forms.TextBox txtSimbolo;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCrear;
    }
}
