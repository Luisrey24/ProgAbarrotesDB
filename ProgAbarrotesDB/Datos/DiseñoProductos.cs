using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Components
{
    public class DiseñoProductos
    {
        private Form formProductos;
        private Label lblTitulo;
        private DataGridView dgvProductos;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblPrecio;
        private TextBox txtPrecio;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private Label lblStock; // Nuevo label para Stock
        private TextBox txtStock; // Nuevo textbox para Stock
        private FlowLayoutPanel panelBotones;
        private Button btnAlta;
        private Button btnConsulta;
        private Button btnModificar;
        private Button btnEliminar;

        public DiseñoProductos(Form form)
        {
            formProductos = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configuración del formulario
            formProductos.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formProductos.Size = new Size(900, 750);
            formProductos.StartPosition = FormStartPosition.CenterScreen;
            formProductos.FormBorderStyle = FormBorderStyle.FixedSingle;
            formProductos.MaximizeBox = false;

            // Título
            lblTitulo = CrearLabel("Productos", new Font("Segoe UI", 20, FontStyle.Bold), new Point(20, 20), Color.FromArgb(190, 100, 220));
            formProductos.Controls.Add(lblTitulo);

            // DataGridView para mostrar los productos
            dgvProductos = CrearDataGridView(new Point(20, 70), new Size(860, 300));
            formProductos.Controls.Add(dgvProductos);

            // Label y TextBox para Nombre
            lblNombre = CrearLabel("Nombre:", new Point(20, 400), Color.White);
            txtNombre = CrearTextBox("txtNombre", new Size(250, 30), new Point(100, 395));
            formProductos.Controls.Add(lblNombre);
            formProductos.Controls.Add(txtNombre);

            // Label y TextBox para Precio
            lblPrecio = CrearLabel("Precio:", new Point(400, 400), Color.White);
            txtPrecio = CrearTextBox("txtPrecio", new Size(250, 30), new Point(460, 395));
            formProductos.Controls.Add(lblPrecio);
            formProductos.Controls.Add(txtPrecio);

            // Label y TextBox para Descripción
            lblDescripcion = CrearLabel("Descripción:", new Point(20, 450), Color.White);
            txtDescripcion = CrearTextBox("txtDescripcion", new Size(700, 30), new Point(130, 445));
            formProductos.Controls.Add(lblDescripcion);
            formProductos.Controls.Add(txtDescripcion);

            // Label y TextBox para Stock
            lblStock = CrearLabel("Stock:", new Point(20, 500), Color.White);
            txtStock = CrearTextBox("txtStock", new Size(250, 30), new Point(100, 495));
            formProductos.Controls.Add(lblStock);
            formProductos.Controls.Add(txtStock);

            // Crear FlowLayoutPanel para organizar los botones
            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(20, 550),
                Size = new Size(860, 80),
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            formProductos.Controls.Add(panelBotones);

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
            dgvProductos.Columns.Add("Producto", "Producto");
            dgvProductos.Columns.Add("Precio", "Precio");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("Stock", "Stock"); // Nueva columna para Stock

            // Ajustar el formato si es necesario
            foreach (DataGridViewColumn column in dgvProductos.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Formatear columnas de precio
            if (dgvProductos.Columns["Precio"] != null)
                dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "C";
        }

        // Propiedades para acceder a los controles desde FormProductos
        public DataGridView DgvProductos => dgvProductos;
        public TextBox TxtNombre => txtNombre;
        public TextBox TxtPrecio => txtPrecio;
        public TextBox TxtDescripcion => txtDescripcion;
        public TextBox TxtStock => txtStock; // Propiedad para el nuevo TextBox
        public Button BtnAlta => btnAlta;
        public Button BtnConsulta => btnConsulta;
        public Button BtnModificar => btnModificar;
        public Button BtnEliminar => btnEliminar;
    }
}
