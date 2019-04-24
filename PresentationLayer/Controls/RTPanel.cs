using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorReportes.PresentationLayer.Controls
{
    public class RTPanel:Panel
    {
        public delegate void ControlClickedHandler(object sender);
        public event ControlClickedHandler ControlClicked;

        private string identificador = string.Empty;
        private string nombre = string.Empty;
        private bool esSeleccionada = false;
        
        public string Identificador
        {
            get { return identificador; }            
        }
        public bool EstaSeleccionada
        {
            get
            {
                return esSeleccionada;
            }
        }
        
        public RTPanel(string identificador, string nombre, int ancho)
            : base()
        {
            base.Width = ancho - 23;
            this.identificador = identificador;
            this.Cursor = Cursors.Hand;

            Label l = new Label();
            l.AutoSize = true;
            l.Text = nombre;
            this.Controls.Add(l);
            l.Dock = DockStyle.Fill;

            this.Height = l.Height + 2;
            if (l.Width > ancho)
                base.Width = l.Width;
            base.BackColor = System.Drawing.Color.FromArgb(220, 230, 240);

            this.Click += new EventHandler(controlClick);
            l.Click += new EventHandler(controlClick);
        }

        void controlClick(object sender, EventArgs e)
        {
            OnControlClicked();
        }

        protected virtual void OnControlClicked()
        {
            if (ControlClicked != null)
            {
                ControlClicked(this);
            }
        }

        public void mover()
        {
            if (esSeleccionada)
                esSeleccionada = false;
            else
                esSeleccionada = true;
        }
    }
}
