using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace UI
{
    public partial class BaseForm : Form
    {
        private LoadingOverlayForm _overlay;
        private bool _isLoading;

        protected virtual Image LoadingGif => Properties.Resources.loader_gif;

        protected virtual string LoadingText => "Cargando datos...";

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => IsLoading = value));
                    return;
                }

                if (_isLoading == value) return;
                _isLoading = value;

                if (_isLoading) ShowLoading();
                else HideLoading();
            }
        }

        public BaseForm()
        {
            DoubleBuffered = true;

            Move += (_, __) => _overlay?.RepositionOver(this);
            Resize += (_, __) => _overlay?.RepositionOver(this);
            ResizeEnd += (_, __) => _overlay?.RepositionOver(this);
            Shown += (_, __) => _overlay?.RepositionOver(this);
            Activated += (_, __) => _overlay?.RepositionOver(this);
        }

        protected virtual void ShowLoading()
        {
            if (_overlay != null) return;

            _overlay = new LoadingOverlayForm(this, LoadingGif, LoadingText)
            {
                Owner = this,
                Location = this.PointToScreen(Point.Empty),
                Size = this.Size,
            };

            Enabled = false;
            UseWaitCursor = true;

            _overlay.Show(this);
            _overlay.RepositionOver(this);
            _overlay.BringToFront();
        }

        protected virtual void HideLoading()
        {
            if (_overlay == null) return;

            try
            {
                _overlay.Hide();
                _overlay.Dispose();
            }
            finally
            {
                _overlay = null;
                Enabled = true;
                UseWaitCursor = false;
                Activate();
            }
        }

        protected async Task RunWithLoading(Func<Task> work)
        {
            IsLoading = true;
            try
            {
                await work().ConfigureAwait(true);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
