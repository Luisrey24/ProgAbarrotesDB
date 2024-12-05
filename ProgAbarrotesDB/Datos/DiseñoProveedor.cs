using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Components
{
    public class DiseñoProveedor
    {
        private Form formProveedor;
        private Label lblTitulo;
        private DataGridView dgvProveedores;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private FlowLayoutPanel panelBotones;
        private Button btnAlta;
        private Button btnConsulta;
        private Button btnModificar;
        private Button btnEliminar;

        public DiseñoProveedor(Form form)
        {
            formProveedor = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configurar el formulario
            formProveedor.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formProveedor.Size = new Size(900, 700);
            formProveedor.StartPosition = FormStartPosition.CenterScreen;
            formProveedor.FormBorderStyle = FormBorderStyle.FixedSingle;
            formProveedor.MaximizeBox = false;

            // Título
            lblTitulo = CrearLabel("Proveedores", new Font("Segoe UI", 20, FontStyle.Bold), new Point(20, 20), Color.FromArgb(190, 100, 220));
            formProveedor.Controls.Add(lblTitulo);

            // DataGridView para mostrar los proveedores
            dgvProveedores = CrearDataGridView(new Point(20, 70), new Size(860, 300));
            formProveedor.Controls.Add(dgvProveedores);

            // Panel para la información de Proveedores
            Panel panelInfo = CrearPanel(new Point(20, 400), new Size(860, 200));
            formProveedor.Controls.Add(panelInfo);

            // Label y TextBox para Nombre
            lblNombre = CrearLabel("Nombre:", new Point(20, 20), Color.White);
            txtNombre = CrearTextBox("txtNombre", new Size(250, 30), new Point(100, 15));
            panelInfo.Controls.Add(lblNombre);
            panelInfo.Controls.Add(txtNombre);

            // Label y TextBox para Teléfono
            lblTelefono = CrearLabel("Teléfono:", new Point(400, 20), Color.White);
            txtTelefono = CrearTextBox("txtTelefono", new Size(250, 30), new Point(480, 15));
            panelInfo.Controls.Add(lblTelefono);
            panelInfo.Controls.Add(txtTelefono);

            // Label y TextBox para Dirección
            lblDireccion = CrearLabel("Dirección:", new Point(20, 80), Color.White);
            txtDireccion = CrearTextBox("txtDireccion", new Size(730, 30), new Point(100, 75));
            panelInfo.Controls.Add(lblDireccion);
            panelInfo.Controls.Add(txtDireccion);

            // Crear FlowLayoutPanel para organizar los botones
            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(20, 620),
                Size = new Size(860, 80),
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            formProveedor.Controls.Add(panelBotones);

            // Botones redondeados organizados en el FlowLayoutPanel
            btnAlta = CrearBotonRedondeado("Alta", new Size(150, 50), Color.FromArgb(106, 90, 205));
            btnConsulta = CrearBotonRedondeado("Consulta", new Size(150, 50), Color.FromArgb(106, 90, 205));
            btnModificar = CrearBotonRedondeado("Modificar", new Size(150, 50), Color.FromArgb(106, 90, 205));
            btnEliminar = CrearBotonRedondeado("Eliminar", new Size(150, 50), Color.FromArgb(106, 90, 205));

            panelBotones.Controls.Add(btnAlta);
            panelBotones.Controls.Add(btnConsulta);
            panelBotones.Controls.Add(btnModificar);
            panelBotones.Controls.Add(btnEliminar);
        }

        // Método para crear etiquetas con estilo
        private Label CrearLabel(string texto, Font font, Point location, Color foreColor)
        {
            return new Label
            {
                Text = texto,
                ForeColor = foreColor,
                Font = font,
                AutoSize = true,
                Location = location
            };
        }

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

        // Método para crear FlowPanels con estilo
        private Panel CrearPanel(Point location, Size size)
        {
            return new Panel
            {
                Location = location,
                Size = size,
                BackColor = Color.FromArgb(45, 0, 80), // Fondo lila fuerte
                BorderStyle = BorderStyle.FixedSingle
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

        // Método para crear DataGridView con estilo
        private DataGridView CrearDataGridView(Point location, Size size)
        {
            DataGridView dgv = new DataGridView
            {
                Location = location,
                Size = size,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(45, 0, 80), // Fondo lila fuerte
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(106, 90, 205),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(45, 0, 80),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(106, 90, 205),
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                GridColor = Color.Gray,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                RowHeadersVisible = false
            };
            dgv.EnableHeadersVisualStyles = false;
            return dgv;
        }

        // Método para configurar las columnas del DataGridView
        private void ConfigurarColumnasDataGridView()
        {
            dgvProveedores.Columns.Add("ID_Proveedor", "ID Proveedor");
            dgvProveedores.Columns.Add("Nombre", "Nombre");
            dgvProveedores.Columns.Add("Telefono", "Teléfono");
            dgvProveedores.Columns.Add("Direccion", "Dirección");

            // Ajustar el formato si es necesario
            // Por ejemplo, centrar el texto
            foreach (DataGridViewColumn column in dgvProveedores.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Ocultar columna ID_Proveedor si no es necesaria
            if (dgvProveedores.Columns.Contains("ID_Proveedor"))
                dgvProveedores.Columns["ID_Proveedor"].Visible = false;
        }

        // Propiedades para acceder a los controles desde FormProveedor
        public DataGridView DgvProveedores => dgvProveedores;
        public TextBox TxtNombre => txtNombre;
        public TextBox TxtTelefono => txtTelefono;
        public TextBox TxtDireccion => txtDireccion;
        public Button BtnAlta => btnAlta;
        public Button BtnConsulta => btnConsulta;
        public Button BtnModificar => btnModificar;
        public Button BtnEliminar => btnEliminar;
    }
}
