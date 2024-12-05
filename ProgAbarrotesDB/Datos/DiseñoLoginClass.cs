using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ProgAbarrotesDB.Clases
{
    public class DiseñoLoginClass
    {
        private Form formLogin;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnIngresar;
        private Button btnCancelar;
        private PictureBox pictureLogo;
        private Panel panelCentral;

        public DiseñoLoginClass(Form form)
        {
            formLogin = form;
            InitializeControles();
            IniciarAnimacion();
        }

        private void InitializeControles()
        {

            // Configurar el formulario
            formLogin.Text = "INICIO DE SESIÓN";
            formLogin.Size = new Size(410, 410);
            formLogin.StartPosition = FormStartPosition.CenterScreen;
            formLogin.FormBorderStyle = FormBorderStyle.FixedSingle;
            formLogin.MaximizeBox = false;
            formLogin.BackColor = Color.FromArgb(147, 112, 219); // Fondo lila más fuerte

            // Panel central para organizar los controles
            panelCentral = new Panel
            {
                Size = new Size(350, 320),
                Location = new Point(-400, 25), // Posición inicial fuera de la vista para la animación
                BackColor = Color.FromArgb(147, 112, 219), // Fondo lila más fuerte
                BorderStyle = BorderStyle.FixedSingle
            };
            formLogin.Controls.Add(panelCentral);

            // Configuración del logotipo
            pictureLogo = new PictureBox
            {
                Image = Properties.Resources.logo, // Asegúrate de agregar la imagen del logo a los recursos
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(100, 100),
                Location = new Point((panelCentral.Width - 100) / 2, 10)
            };
            panelCentral.Controls.Add(pictureLogo);

            // Etiqueta de Usuario
            Label lblUsuario = CrearLabel("Usuario:", new Point(50, 130), Color.White);
            panelCentral.Controls.Add(lblUsuario);

            // Cuadro de texto para el Usuario
            txtUsuario = CrearTextBox("txtUsuario", new Size(250, 30), new Point(50, 150));
            panelCentral.Controls.Add(txtUsuario);

            // Etiqueta de Contraseña
            Label lblContrasena = CrearLabel("Contraseña:", new Point(50, 190), Color.White);
            panelCentral.Controls.Add(lblContrasena);

            // Cuadro de texto para la Contraseña
            txtContrasena = CrearTextBox("txtContrasena", new Size(250, 30), new Point(50, 210));
            txtContrasena.UseSystemPasswordChar = true;
            panelCentral.Controls.Add(txtContrasena);

            // Panel para los botones
            FlowLayoutPanel panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(50, 260),
                Size = new Size(250, 50),
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            panelCentral.Controls.Add(panelBotones);

            // Botón Ingresar
            btnIngresar = CrearBotonRedondeado("Ingresar", new Size(110, 40), Color.FromArgb(106, 90, 205));
            panelBotones.Controls.Add(btnIngresar);

            // Botón Cancelar
            btnCancelar = CrearBotonRedondeado("Cancelar", new Size(110, 40), Color.FromArgb(106, 90, 205));
            panelBotones.Controls.Add(btnCancelar);
        }

        // Método para crear etiquetas con estilo
        private Label CrearLabel(string texto, Point location, Color foreColor)
        {
            return new Label
            {
                Text = texto,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = location
            };
        }

        // Método para crear TextBoxes con estilo
        private TextBox CrearTextBox(string name, Size size, Point location)
        {
            return new TextBox
            {
                Name = name,
                Size = size,
                Location = location,
                BackColor = Color.FromArgb(106, 90, 205), // Fondo lila más oscuro
                ForeColor = Color.White, // Texto en blanco para contraste
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
        }

        // Método para crear botones redondeados con estilo
        private Button CrearBotonRedondeado(string texto, Size size, Color backColor)
        {
            Button btn = new Button
            {
                Text = texto,
                Size = size,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = backColor,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(10) // Espaciado entre los botones en el FlowLayoutPanel
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 20, 20));

            // Efecto Hover
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(123, 104, 238);
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;

            return btn;
        }

        // Importar método para crear bordes redondeados
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        // Método de animación para deslizar el panel central
        private async void IniciarAnimacion()
        {
            int targetX = 25; // Posición objetivo para el panel
            while (panelCentral.Location.X < targetX)
            {
                panelCentral.Location = new Point(panelCentral.Location.X + 10, panelCentral.Location.Y);
                await Task.Delay(10); // Retraso para suavizar la animación
            }
            panelCentral.Location = new Point(targetX, panelCentral.Location.Y); // Asegurar la posición final
        }

        // Métodos para obtener los valores de los cuadros de texto
        public string ObtenerUsuario()
        {
            return txtUsuario.Text.Trim();
        }

        public string ObtenerContrasena()
        {
            return txtContrasena.Text;
        }

        // Métodos para asignar eventos a los botones
        public void AsignarEventoIngresar(EventHandler evento)
        {
            btnIngresar.Click += evento;
        }

        public void AsignarEventoCancelar(EventHandler evento)
        {
            btnCancelar.Click += evento;
        }

        public void RellenarInicio()
        {
            txtUsuario.Text = "sa";
            txtContrasena.Text = "1234";
        }

        // Método para limpiar los campos de texto
        public void LimpiarCampos()
        {
           
        }
    }
}
