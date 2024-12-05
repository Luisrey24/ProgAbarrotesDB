using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgAbarrotesDB
{
    public class DiseñoReporteVentas
    {
        private Form formReporteVentas;
        private Label lblTitulo;
        private Label lblFecha;
        private DateTimePicker dtpFecha;
        private Label lblCliente;
        private ComboBox cmbCliente;
        private Button btnGenerarReporte;
        private DataGridView dgvReporteVentas;

        public DiseñoReporteVentas(Form form)
        {
            formReporteVentas = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configuración del formulario
            formReporteVentas.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formReporteVentas.Size = new Size(1000, 700);
            formReporteVentas.StartPosition = FormStartPosition.CenterScreen;
            formReporteVentas.FormBorderStyle = FormBorderStyle.FixedSingle;
            formReporteVentas.MaximizeBox = false;

            // Título
            lblTitulo = CrearLabel("Reporte de Ventas", new Font("Segoe UI", 18, FontStyle.Bold), new Point(20, 20), Color.FromArgb(190, 100, 220));
            formReporteVentas.Controls.Add(lblTitulo);

            // Panel para Filtros
            Panel panelFiltros = CrearPanel(new Point(20, 70), new Size(940, 50));
            formReporteVentas.Controls.Add(panelFiltros);

            // Label y DateTimePicker para Fecha
            lblFecha = CrearLabel("Fecha:", new Point(0, 15), Color.White);
            panelFiltros.Controls.Add(lblFecha);

            dtpFecha = CrearDateTimePicker(new Point(60, 10));
            panelFiltros.Controls.Add(dtpFecha);

            // Label y ComboBox para Cliente
            lblCliente = CrearLabel("Cliente:", new Point(300, 15), Color.White);
            panelFiltros.Controls.Add(lblCliente);

            cmbCliente = CrearComboBox("cmbCliente", new Size(220, 25), new Point(370, 10));
            panelFiltros.Controls.Add(cmbCliente);

            // Botón Generar Reporte (redondeado)
            btnGenerarReporte = CrearBotonRedondeado("Generar Reporte", new Size(150, 40), new Point(650, 5), Color.FromArgb(150, 85, 190));
            panelFiltros.Controls.Add(btnGenerarReporte);

            // DataGridView para Reporte de Ventas
            dgvReporteVentas = CrearDataGridView(new Point(20, 130), new Size(940, 460));
            formReporteVentas.Controls.Add(dgvReporteVentas);

            // Configurar Columnas del DataGridView
            ConfigurarColumnasDataGridView();
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

        // Método para crear DateTimePicker con estilo
        private DateTimePicker CrearDateTimePicker(Point location)
        {
            return new DateTimePicker
            {
                Name = "dtpFecha",
                Size = new Size(220, 25),
                Location = location,
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = Color.White,
                CalendarMonthBackground = Color.FromArgb(106, 90, 205),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
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

        // Método para crear botones redondeados con estilo
        private Button CrearBotonRedondeado(string texto, Size size, Point location, Color backColor)
        {
            Button btn = new Button
            {
                Text = texto,
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = backColor,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 20, 20));

            // Efecto Hover
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(180, 100, 220);
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;

            return btn;
        }

        // Método para crear Panel con estilo
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
            dgvReporteVentas.Columns.Add("Cliente", "Cliente");
            dgvReporteVentas.Columns.Add("Fecha", "Fecha");
            dgvReporteVentas.Columns.Add("Producto", "Producto");
            dgvReporteVentas.Columns.Add("Cantidad", "Cantidad");
            dgvReporteVentas.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvReporteVentas.Columns.Add("Total", "Total");
            dgvReporteVentas.Columns.Add("MetodoPago", "Método de Pago");

            // Ocultar columna ID_Cliente si es necesario
            DataGridViewColumn columnIDCliente = new DataGridViewTextBoxColumn
            {
                Name = "ID_Cliente",
                Visible = false
            };
            dgvReporteVentas.Columns.Add(columnIDCliente);

            dgvReporteVentas.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C";
            dgvReporteVentas.Columns["Total"].DefaultCellStyle.Format = "C";

            // Ajuste de estilos adicionales
            dgvReporteVentas.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReporteVentas.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Propiedades para acceder a los controles desde FormReporteVentas
        public DateTimePicker DtpFecha => dtpFecha;
        public ComboBox CmbCliente => cmbCliente;
        public Button BtnGenerarReporte => btnGenerarReporte;
        public DataGridView DgvReporteVentas => dgvReporteVentas;
    }
}
