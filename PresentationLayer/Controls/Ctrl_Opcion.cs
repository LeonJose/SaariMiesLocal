using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer.Controls
{
    public partial class Ctrl_Opcion : UserControl
    {
        public Color FondoColor { get; set; }
        [Description("Texto que será visible en el control"), Category("Default")]
        public string Texto
        {
            get
            {
                return labelText.Text;
            }
            set
            {
                labelText.Text = value;
            }
        }
        [Description("Imagen que será visible en el control"), Category("Default")]
        public Image Imagen
        {
            get
            {
                return picImage.Image;
            }
            set
            {
                picImage.Image = value;
            }
        }
        public delegate void ControlClickedHandler(object sender, EventArgs e);
        public event ControlClickedHandler ControlClicked;

        private Image imagenOriginal = null;
        private Image imagenBN = null;

        public Ctrl_Opcion()
        {
            InitializeComponent();
            FondoColor = BackColor;
            this.Click += new EventHandler(controlClick);
            labelText.Click += new EventHandler(controlClick);
            picImage.Click += new EventHandler(controlClick);
            tableLayoutPanel1.Click += new EventHandler(controlClick);
            this.MouseEnter += new EventHandler(controlMouseEnter);
            labelText.MouseEnter += new EventHandler(controlMouseEnter);
            picImage.MouseEnter += new EventHandler(controlMouseEnter);
            tableLayoutPanel1.MouseEnter += new EventHandler(controlMouseEnter);
            this.MouseLeave += new EventHandler(controlMouseLeave);
            labelText.MouseLeave += new EventHandler(controlMouseLeave);
            picImage.MouseLeave += new EventHandler(controlMouseLeave);
            tableLayoutPanel1.MouseLeave += new EventHandler(controlMouseLeave);
        }

        void controlMouseLeave(object sender, EventArgs e)
        {
            BackColor = FondoColor;
        }

        void controlMouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.Orange;
        }

        void controlClick(object sender, EventArgs e)
        {
            OnControlClicked();
        }

        protected virtual void OnControlClicked()
        {
            var handler = ControlClicked;
            if (handler != null)
                handler(this, new EventArgs());
        }

        private void Ctrl_Opcion_EnabledChanged(object sender, EventArgs e)
        {
            if (imagenOriginal == null)
                imagenOriginal = Imagen;
            if (imagenOriginal != null)
            {
                if (imagenBN == null)
                {
                    imagenBN = createGrayScaleImage(imagenOriginal);
                }
            }

            if (imagenBN != null)
            {
                if (this.Enabled)
                    picImage.Image = imagenOriginal;
                else
                    picImage.Image = imagenBN;
            }
        }

        private Image createGrayScaleImage(Image source)
        {
            try
            {
                int rgb = 0;
                Color c;
                Bitmap btmSource = new Bitmap(source);
                for (int y = 0; y < btmSource.Height; y++)
                {
                    for (int x = 0; x < btmSource.Width; x++)
                    {
                        c = btmSource.GetPixel(x, y);
                        rgb = (int)(.299 * c.R + .587 * c.G + .114 * c.B);
                        btmSource.SetPixel(x, y, Color.FromArgb(c.A, rgb, rgb, rgb));
                    }
                }
                return btmSource;
            }
            catch 
            {
                return null;
            }
        }

        private void Ctrl_Opcion_Enter(object sender, EventArgs e)
        {
            BackColor = Color.Orange;
        }

        private void Ctrl_Opcion_Leave(object sender, EventArgs e)
        {
            BackColor = FondoColor;
        }
    }
}
