using System;
using System.Drawing;
using System.Windows.Forms;

namespace UI
{
    internal partial class LoadingOverlayForm : Form
    {
        private readonly PictureBox _gif;
        private readonly Label _label;

        // El color llave de transparencia
        private static readonly Color TransparentKeyColor = Color.Magenta;

        public LoadingOverlayForm(Form owner, Image gifImage, string mensaje = "Cargando datos...")
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = owner.TopMost;
            BackColor = TransparentKeyColor;
            TransparencyKey = TransparentKeyColor;

            Size = owner.Size;


            _gif = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.AutoSize,
                Image = gifImage,
                BackColor = Color.Transparent
            };

            _label = new Label
            {
                AutoSize = true,
                Text = mensaje,
                Font = new Font(SystemFonts.DialogFont.FontFamily, 11f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(180, 0, 0, 0), 
                Padding = new Padding(10, 6, 10, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(_gif);
            Controls.Add(_label);

            DoubleBuffered = true;

            Activated += (s, e) => owner?.Activate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                cp.ExStyle |= 0x80 | 0x20;
                return cp;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            using (var brush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (_gif.Image != null)
            {
                _gif.Left = (ClientSize.Width - _gif.Width) / 2;
                _gif.Top = (ClientSize.Height - _gif.Height) / 2 - 16;
            }

            _label.Left = (ClientSize.Width - _label.Width) / 2;
            _label.Top = _gif.Bottom + 10;
        }

        public void RepositionOver(Form owner)
        {
            if (owner == null) return;
            Location = owner.PointToScreen(Point.Empty);
            Size = owner.Size;
            PerformLayout();
        }
    }
}
