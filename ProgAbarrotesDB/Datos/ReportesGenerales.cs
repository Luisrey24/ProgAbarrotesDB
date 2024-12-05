using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProgAbarrotesDB
{
    public class ReportesGenerales
    {
        private Form formReporte;
        private Label lblTitulo;

        // Controles para Clientes
        private Label lblCliente;
        private ComboBox cmbClientes;
        private DataGridView dgvClientes;

        // Controles para Proveedores
        private Label lblProveedor;
        private ComboBox cmbProveedores;
        private DataGridView dgvProveedores;

        // Controles para Productos
        private Label lblProducto;
        private ComboBox cmbProductos;
        private DataGridView dgvProductos;

        // TableLayoutPanel para organizar los paneles
        private TableLayoutPanel tableLayout;

        public ReportesGenerales(Form form)
        {
            formReporte = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configuración del formulario
            formReporte.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formReporte.Size = new Size(1200, 900); // Aumentar el tamaño para mejor acomodación
            formReporte.StartPosition = FormStartPosition.CenterScreen;
            formReporte.FormBorderStyle = FormBorderStyle.FixedSingle;
            formReporte.MaximizeBox = false;

            // Título
            lblTitulo = CrearLabel("Reportes Generales de Clientes, Proveedores y Productos", new Font("Segoe UI", 20, FontStyle.Bold), new Point(20, 20), Color.FromArgb(190, 100, 220));
            formReporte.Controls.Add(lblTitulo);

            // Configurar TableLayoutPanel
            tableLayout = new TableLayoutPanel
            {
                Location = new Point(20, 70),
                Size = new Size(1140, 800),
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.FromArgb(45, 0, 80),
                AutoSize = true
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            formReporte.Controls.Add(tableLayout);

            // Sección de Clientes
            Panel panelClientes = CrearPanel(new Size(550, 380));
            tableLayout.Controls.Add(panelClientes, 0, 0);

            lblCliente = CrearLabel("Cliente:", new Point(10, 10), Color.White);
            panelClientes.Controls.Add(lblCliente);

            cmbClientes = CrearComboBox("cmbClientes", new Size(200, 25), new Point(80, 5));
            panelClientes.Controls.Add(cmbClientes);

            dgvClientes = CrearDataGridView(new Point(0, 50), new Size(550, 320));
            panelClientes.Controls.Add(dgvClientes);

            // Configurar Columnas del DataGridView de Clientes
            ConfigurarColumnasDataGridView(dgvClientes, new string[] { "Nombre", "Telefono", "Direccion" });

            // Sección de Proveedores
            Panel panelProveedores = CrearPanel(new Size(550, 380));
            tableLayout.Controls.Add(panelProveedores, 1, 0);

            lblProveedor = CrearLabel("Proveedor:", new Point(10, 10), Color.White);
            panelProveedores.Controls.Add(lblProveedor);

            cmbProveedores = CrearComboBox("cmbProveedores", new Size(200, 25), new Point(100, 5));
            panelProveedores.Controls.Add(cmbProveedores);

            dgvProveedores = CrearDataGridView(new Point(0, 50), new Size(550, 320));
            panelProveedores.Controls.Add(dgvProveedores);

            // Configurar Columnas del DataGridView de Proveedores
            ConfigurarColumnasDataGridView(dgvProveedores, new string[] { "Nombre", "Telefono", "Direccion" });

            // Sección de Productos
            Panel panelProductos = CrearPanel(new Size(550, 380));
            tableLayout.Controls.Add(panelProductos, 0, 1);

            lblProducto = CrearLabel("Producto:", new Point(10, 10), Color.White);
            panelProductos.Controls.Add(lblProducto);

            cmbProductos = CrearComboBox("cmbProductos", new Size(200, 25), new Point(90, 5));
            panelProductos.Controls.Add(cmbProductos);

            dgvProductos = CrearDataGridView(new Point(0, 50), new Size(550, 320));
            panelProductos.Controls.Add(dgvProductos);

            // Configurar Columnas del DataGridView de Productos
            ConfigurarColumnasDataGridView(dgvProductos, new string[] { "Producto", "Precio", "Descripcion" });

            // Espacio adicional o panel vacío en la segunda columna de la segunda fila
            Panel panelVacio = CrearPanel(new Size(550, 380));
            tableLayout.Controls.Add(panelVacio, 1, 1);
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

        // Método para crear ComboBox con estilo
        private ComboBox CrearComboBox(string name, Size size, Point location)
        {
            return new ComboBox
            {
                Name = name,
                Size = size,
                Location = location,
                BackColor = Color.FromArgb(106, 90, 205), // Fondo lila más oscuro
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
        }

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
        private void ConfigurarColumnasDataGridView(DataGridView dgv, string[] columnas)
        {
            dgv.Columns.Clear(); // Limpiar columnas existentes si las hay
            foreach (var columna in columnas)
            {
                dgv.Columns.Add(columna, columna);
            }

            // Ajustar el formato si es necesario
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Formatear columnas de precio si existen
            if (dgv.Columns.Contains("Precio"))
                dgv.Columns["Precio"].DefaultCellStyle.Format = "C";
        }

        // Método para crear Panel con estilo
        private Panel CrearPanel(Size size)
        {
            return new Panel
            {
                Size = size,
                BackColor = Color.FromArgb(45, 0, 80), // Fondo lila fuerte
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        // Propiedades para acceder a los controles desde el formulario
        public ComboBox CmbClientes => cmbClientes;
        public DataGridView DgvClientes => dgvClientes;

        public ComboBox CmbProveedores => cmbProveedores;
        public DataGridView DgvProveedores => dgvProveedores;

        public ComboBox CmbProductos => cmbProductos;
        public DataGridView DgvProductos => dgvProductos;
    }
}
