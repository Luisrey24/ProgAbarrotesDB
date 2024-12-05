using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Components
{
    public class DiseñoVentas
    {
        private Form formVentas;

        // Título
        private Label lblTitulo;

        // Fila 1: Cliente y Fecha de Compra
        private Label lblCliente;
        private ComboBox cmbCliente;
        private Label lblFechaCompra;
        private DateTimePicker dtpFechaCompra;

        // Fila 2: Producto y Cantidad
        private Label lblProducto;
        private ComboBox cmbProducto;
        private Label lblCantidad;
        private TextBox txtCantidad;

        // Fila 3: Precio por Unidad y Método de Pago
        private Label lblPrecioPorUnidad;
        private Label lblCostoUnidad;
        private Label lblMetodoPago;
        private ComboBox cmbMetodoPago;

        // Botones: Agregar, Consultar, Eliminar
        private FlowLayoutPanel panelBotones;
        private Button btnAgregar;
        private Button btnConsultar;
        private Button btnEliminar;

        // DataGridView para Ventas
        private DataGridView dgvVentas;

        // Botón Registrar
        private Button btnRegistrar;

        // Totales: SubTotal, IVA y Total
        private FlowLayoutPanel panelTotales;
        private Label lblSubTotalText;
        private Label lblSubTotalValue;
        private Label lblIvaText;
        private Label lblIvaValue;
        private Label lblTotalText;
        private Label lblTotalValue;

        public DiseñoVentas(Form form)
        {
            formVentas = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configuración del formulario
            formVentas.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formVentas.Size = new Size(920, 750);
            formVentas.StartPosition = FormStartPosition.CenterScreen;
            formVentas.FormBorderStyle = FormBorderStyle.FixedSingle;
            formVentas.MaximizeBox = false;

            // Crear el TableLayoutPanel principal
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                RowCount = 6,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoScroll = true
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // Título
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // Cliente y Fecha
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // Producto y Cantidad
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F)); // Precio y Método de Pago
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F)); // Botones y Totales
            formVentas.Controls.Add(mainLayout);

            // ------------------- Título -------------------
            lblTitulo = CrearLabel("Ventas", new Font("Segoe UI", 20, FontStyle.Bold), Color.FromArgb(190, 100, 220));
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            mainLayout.Controls.Add(lblTitulo, 0, 0);

            // ------------------- Cliente y Fecha de Compra -------------------
            TableLayoutPanel row1 = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 4,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true
            };
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Labels
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Controls
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Labels
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Controls

            lblCliente = CrearLabel("Cliente:", new Point(0, 0), Color.White);
            cmbCliente = CrearComboBox("cmbCliente");
            lblFechaCompra = CrearLabel("Fecha de Compra:", new Point(0, 0), Color.White);
            dtpFechaCompra = CrearDateTimePicker();

            row1.Controls.Add(lblCliente, 0, 0);
            row1.Controls.Add(cmbCliente, 1, 0);
            row1.Controls.Add(lblFechaCompra, 2, 0);
            row1.Controls.Add(dtpFechaCompra, 3, 0);

            mainLayout.Controls.Add(row1, 0, 1);

            // ------------------- Producto y Cantidad -------------------
            TableLayoutPanel row2 = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 4,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true
            };
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            lblProducto = CrearLabel("Producto:", new Point(0, 0), Color.White);
            cmbProducto = CrearComboBox("cmbProducto");
            lblCantidad = CrearLabel("Cantidad:", new Point(0, 0), Color.White);
            txtCantidad = CrearTextBox("txtCantidad");

            row2.Controls.Add(lblProducto, 0, 0);
            row2.Controls.Add(cmbProducto, 1, 0);
            row2.Controls.Add(lblCantidad, 2, 0);
            row2.Controls.Add(txtCantidad, 3, 0);

            mainLayout.Controls.Add(row2, 0, 2);

            // ------------------- Precio por Unidad y Método de Pago -------------------
            TableLayoutPanel row3 = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 4,
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true
            };
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            lblPrecioPorUnidad = CrearLabel("Precio por Unidad:", new Point(0, 0), Color.White);
            lblCostoUnidad = CrearLabel("$0.00", new Font("Segoe UI", 12, FontStyle.Bold), Color.White);
            lblMetodoPago = CrearLabel("Método de Pago:", new Point(0, 0), Color.White);
            cmbMetodoPago = CrearComboBox("cmbMetodoPago");

            row3.Controls.Add(lblPrecioPorUnidad, 0, 0);
            row3.Controls.Add(lblCostoUnidad, 1, 0);
            row3.Controls.Add(lblMetodoPago, 2, 0);
            row3.Controls.Add(cmbMetodoPago, 3, 0);

            mainLayout.Controls.Add(row3, 0, 3);

            // ------------------- Botones Agregar, Consultar, Eliminar -------------------
            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(0)
            };
            btnAgregar = CrearBotonRedondeado("Agregar", new Size(150, 40), Color.FromArgb(106, 90, 205));
            btnConsultar = CrearBotonRedondeado("Consultar", new Size(150, 40), Color.FromArgb(106, 90, 205));
            btnEliminar = CrearBotonRedondeado("Eliminar Seleccionado", new Size(180, 40), Color.FromArgb(106, 90, 205));

            panelBotones.Controls.Add(btnAgregar);
            panelBotones.Controls.Add(btnConsultar);
            panelBotones.Controls.Add(btnEliminar);

            mainLayout.Controls.Add(panelBotones, 0, 4);

            // ------------------- DataGridView para Ventas -------------------
            dgvVentas = CrearDataGridView();
            mainLayout.Controls.Add(dgvVentas, 0, 5);

            // ------------------- Botón Registrar -------------------
            btnRegistrar = CrearBotonRedondeado("Registrar", new Size(150, 40), Color.FromArgb(106, 90, 205));
            mainLayout.Controls.Add(btnRegistrar, 0, 6);

            // ------------------- Panel para SubTotal, IVA y Total -------------------
            panelTotales = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Padding = new Padding(10),
                Margin = new Padding(0)
            };

            lblSubTotalText = CrearLabel("Sub Total:", new Point(0, 0), Color.White);
            lblSubTotalValue = CrearLabel("$0.00", new Font("Segoe UI", 10, FontStyle.Bold), Color.FromArgb(190, 100, 220));

            lblIvaText = CrearLabel("IVA (16%):", new Point(20, 0), Color.White);
            lblIvaValue = CrearLabel("$0.00", new Font("Segoe UI", 10, FontStyle.Bold), Color.FromArgb(190, 100, 220));

            lblTotalText = CrearLabel("Total:", new Point(20, 0), Color.White);
            lblTotalValue = CrearLabel("$0.00", new Font("Segoe UI", 10, FontStyle.Bold), Color.FromArgb(190, 100, 220));

            panelTotales.Controls.Add(lblSubTotalText);
            panelTotales.Controls.Add(lblSubTotalValue);
            panelTotales.Controls.Add(lblIvaText);
            panelTotales.Controls.Add(lblIvaValue);
            panelTotales.Controls.Add(lblTotalText);
            panelTotales.Controls.Add(lblTotalValue);

            mainLayout.Controls.Add(panelTotales, 0, 7);
        }

        #region Métodos de Creación de Controles

        private Label CrearLabel(string texto, Point location, Color foreColor)
        {
            return new Label
            {
                Text = texto,
                ForeColor = foreColor,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 10, 10, 0)
            };
        }

        private Label CrearLabel(string texto, Font font, Color foreColor)
        {
            return new Label
            {
                Text = texto,
                ForeColor = foreColor,
                Font = font,
                AutoSize = true,
                Margin = new Padding(0, 10, 10, 0)
            };
        }

        private ComboBox CrearComboBox(string name)
        {
            return new ComboBox
            {
                Name = name,
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(106, 90, 205), // Fondo lila más oscuro
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 0)
            };
        }

        private TextBox CrearTextBox(string name)
        {
            return new TextBox
            {
                Name = name,
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(106, 90, 205), // Fondo lila más oscuro
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 5, 10, 0)
            };
        }

        private DateTimePicker CrearDateTimePicker()
        {
            return new DateTimePicker
            {
                Name = "dtpFechaCompra",
                Size = new Size(200, 25),
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = Color.FromArgb(106, 90, 205),
                CalendarMonthBackground = Color.FromArgb(106, 90, 205),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 5, 10, 0)
            };
        }

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

        private DataGridView CrearDataGridView()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(45, 0, 80), // Fondo lila fuerte
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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

            // Configurar columnas
            ConfigurarColumnasDataGridView(dgv);

            return dgv;
        }

        private void ConfigurarColumnasDataGridView(DataGridView dgv)
        {
            dgv.Columns.Add("Cliente", "Cliente");
            dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Producto", "Producto");
            dgv.Columns.Add("Cantidad", "Cantidad");
            dgv.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgv.Columns.Add("Total", "Total");
            dgv.Columns.Add("MetodoPago", "Método de Pago");

            // Formatear columnas de precio y total
            if (dgv.Columns.Contains("PrecioUnitario"))
                dgv.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C";

            if (dgv.Columns.Contains("Total"))
                dgv.Columns["Total"].DefaultCellStyle.Format = "C";

            // Ajustar el formato si es necesario
            // Por ejemplo, centrar el texto
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        #endregion

        #region Propiedades para Acceso desde FormVentas

        public ComboBox CmbCliente => cmbCliente;
        public DateTimePicker DtpFechaCompra => dtpFechaCompra;
        public ComboBox CmbProducto => cmbProducto;
        public TextBox TxtCantidad => txtCantidad;
        public Label LblCostoUnidad => lblCostoUnidad;
        public ComboBox CmbMetodoPago => cmbMetodoPago;
        public Button BtnAgregar => btnAgregar;
        public Button BtnConsultar => btnConsultar;
        public Button BtnRegistrar => btnRegistrar;
        public Button BtnEliminar => btnEliminar;
        public DataGridView DgvVentas => dgvVentas;

        public Label LblSubTotalValue => lblSubTotalValue;
        public Label LblIvaValue => lblIvaValue;
        public Label LblTotalValue => lblTotalValue;

        #endregion
    }
}
