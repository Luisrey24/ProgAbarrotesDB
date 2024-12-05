using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Components
{
    public class DiseñoInventario
    {
        private Form formInventario;
        private Label lblTitulo;

        private Label lblProducto;
        private ComboBox cmbProducto;

        private Label lblProveedor;
        private ComboBox cmbProveedor;

        private Label lblFecha;
        private DateTimePicker dtpFecha;

        private Label lblObservaciones;
        private TextBox txtObservaciones;

        private Label lblCantidadEntrada;
        private TextBox txtCantidadEntrada;

        private Label lblStockDisponible;

        private FlowLayoutPanel panelBotones;
        private Button btnAgregar;
        private Button btnAñadirInventario;
        private Button btnLimpiar;

        private DataGridView dgvInventario;

        private Label lblSubtotal;
        private Label lblIVA;
        private Label lblTotal;

        public DiseñoInventario(Form form)
        {
            formInventario = form;
            InitializeControles();
        }

        private void InitializeControles()
        {
            // Configuración del formulario
            formInventario.BackColor = Color.FromArgb(45, 0, 80); // Fondo lila fuerte
            formInventario.Size = new Size(920, 750);
            formInventario.StartPosition = FormStartPosition.CenterScreen;
            formInventario.FormBorderStyle = FormBorderStyle.FixedSingle;
            formInventario.MaximizeBox = false;

            // Título
            lblTitulo = CrearLabel("Inventario", new Font("Segoe UI", 20, FontStyle.Bold), new Point(20, 20), Color.FromArgb(190, 100, 220));
            formInventario.Controls.Add(lblTitulo);

            // Panel para la información de Inventario
            Panel panelInfo = new Panel
            {
                Location = new Point(20, 70),
                Size = new Size(860, 200),
                BackColor = Color.FromArgb(45, 0, 80), // Fondo lila fuerte
                BorderStyle = BorderStyle.FixedSingle
            };
            formInventario.Controls.Add(panelInfo);

            // Producto
            lblProducto = CrearLabel("Producto:", new Point(0, 0), Color.White);
            cmbProducto = CrearComboBox("cmbProducto", new Size(250, 30), new Point(0, 30));
            panelInfo.Controls.Add(lblProducto);
            panelInfo.Controls.Add(cmbProducto);

            // Proveedor
            lblProveedor = CrearLabel("Proveedor:", new Point(300, 0), Color.White);
            cmbProveedor = CrearComboBox("cmbProveedor", new Size(250, 30), new Point(300, 30));
            panelInfo.Controls.Add(lblProveedor);
            panelInfo.Controls.Add(cmbProveedor);

            // Fecha
            lblFecha = CrearLabel("Fecha:", new Point(0, 80), Color.White);
            dtpFecha = CrearDateTimePicker(new Point(0, 110));
            panelInfo.Controls.Add(lblFecha);
            panelInfo.Controls.Add(dtpFecha);

            // Observaciones
            lblObservaciones = CrearLabel("Observaciones:", new Point(300, 80), Color.White);
            txtObservaciones = CrearTextBox("txtObservaciones", new Size(250, 30), new Point(300, 110));
            panelInfo.Controls.Add(lblObservaciones);
            panelInfo.Controls.Add(txtObservaciones);

            // Cantidad Entrante
            lblCantidadEntrada = CrearLabel("Cantidad Entrante:", new Point(0, 160), Color.White);
            txtCantidadEntrada = CrearTextBox("txtCantidadEntrada", new Size(250, 30), new Point(150, 160));
            panelInfo.Controls.Add(lblCantidadEntrada);
            panelInfo.Controls.Add(txtCantidadEntrada);

            // Stock Disponible
            lblStockDisponible = CrearLabel("Stock Disponible: N/A", new Point(400, 160), Color.White); // Ajuste de posición
            panelInfo.Controls.Add(lblStockDisponible);

            // Crear FlowLayoutPanel para organizar los botones
            panelBotones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(20, 280),
                Size = new Size(860, 80),
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            formInventario.Controls.Add(panelBotones);

            // Botones redondeados organizados en el FlowLayoutPanel
            btnAgregar = CrearBotonRedondeado("Agregar", new Size(200, 50), Color.FromArgb(106, 90, 205));
            btnAñadirInventario = CrearBotonRedondeado("Añadir al Inventario", new Size(200, 50), Color.FromArgb(106, 90, 205));
            btnLimpiar = CrearBotonRedondeado("Limpiar Campos", new Size(200, 50), Color.FromArgb(106, 90, 205));

            panelBotones.Controls.Add(btnAgregar);
            panelBotones.Controls.Add(btnAñadirInventario);
            panelBotones.Controls.Add(btnLimpiar);

            // DataGridView
            dgvInventario = CrearDataGridView(new Point(20, 380), new Size(860, 200));
            formInventario.Controls.Add(dgvInventario);

            // Labels para Subtotal, IVA y Total
            lblSubtotal = CrearLabel("Subtotal: $0.00", new Font("Segoe UI", 12, FontStyle.Bold), new Point(20, 600), Color.FromArgb(190, 100, 220));
            lblIVA = CrearLabel("IVA (16%): $0.00", new Font("Segoe UI", 12, FontStyle.Bold), new Point(300, 600), Color.FromArgb(190, 100, 220));
            lblTotal = CrearLabel("Total: $0.00", new Font("Segoe UI", 12, FontStyle.Bold), new Point(580, 600), Color.FromArgb(190, 100, 220));
            formInventario.Controls.Add(lblSubtotal);
            formInventario.Controls.Add(lblIVA);
            formInventario.Controls.Add(lblTotal);

            // Agregar columnas al DataGridView
            ConfigurarColumnasDataGridView();
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

        private DateTimePicker CrearDateTimePicker(Point location)
        {
            return new DateTimePicker
            {
                Name = "dtpFecha",
                Size = new Size(250, 30),
                Location = location,
                Format = DateTimePickerFormat.Short,
                CalendarForeColor = Color.FromArgb(106, 90, 205),
                CalendarMonthBackground = Color.FromArgb(106, 90, 205),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
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

        private void ConfigurarColumnasDataGridView()
        {
            dgvInventario.Columns.Add("Producto", "Producto");
            dgvInventario.Columns.Add("Proveedor", "Proveedor");
            dgvInventario.Columns.Add("Fecha", "Fecha");
            dgvInventario.Columns.Add("CantidadEntrada", "Cantidad Entrada");
            dgvInventario.Columns.Add("PrecioUnitario", "Precio por U/.");
            dgvInventario.Columns.Add("Total", "Total");
            dgvInventario.Columns.Add("Observaciones", "Observaciones");

            // Ajustar el formato si es necesario
            // Por ejemplo, centrar el texto
            foreach (DataGridViewColumn column in dgvInventario.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // Formatear columnas de precio y total
            if (dgvInventario.Columns["PrecioUnitario"] != null)
                dgvInventario.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C";

            if (dgvInventario.Columns["Total"] != null)
                dgvInventario.Columns["Total"].DefaultCellStyle.Format = "C";
        }

        // Propiedades para acceder a los controles desde FormInventario
        public ComboBox CmbProducto => cmbProducto;
        public ComboBox CmbProveedor => cmbProveedor;
        public DateTimePicker DtpFecha => dtpFecha;
        public TextBox TxtObservaciones => txtObservaciones;
        public TextBox TxtCantidadEntrada => txtCantidadEntrada;
        public Button BtnAgregar => btnAgregar;
        public Button BtnAñadirInventario => btnAñadirInventario;
        public Button BtnLimpiar => btnLimpiar;
        public DataGridView DgvInventario => dgvInventario;
        public Label LblStockDisponible => lblStockDisponible;
        public Label LblSubtotal => lblSubtotal;
        public Label LblIVA => lblIVA;
        public Label LblTotal => lblTotal;
    }
}
