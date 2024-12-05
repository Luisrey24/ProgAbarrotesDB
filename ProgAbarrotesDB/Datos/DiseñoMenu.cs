using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Datos
{
    internal class DiseñoMenu
    {
        private Form formPrincipal;
        private Panel panelMenu;
        private Timer animationTimer;
        private Timer collapseTimer;
        private bool isMenuExpanded;
        private const int expandedWidth = 220;
        private const int collapsedWidth = 60;
        private int targetWidth;

        private Button btnClientes;
        private Button btnProductos;
        private Button btnProveedor;
        private Button btnVentas;
        private Button btnInventario;
        private Button btnReportesCPP;
        private Button btnReportesVentas;
        private Button btnReportesInventario;
        private Button btnAnalisisVentas; // Nuevo botón
        private Button btnSalir;

        public DiseñoMenu(Form form)
        {
            formPrincipal = form;
            InitializeMenu();
            InitializeAnimation();
            InitializeCollapseTimer();
        }

        private void InitializeMenu()
        {
            // Crear el panel del menú
            panelMenu = new Panel
            {
                Size = new Size(collapsedWidth, formPrincipal.ClientSize.Height),
                Location = new Point(formPrincipal.ClientSize.Width - collapsedWidth, 0),
                BackColor = Color.FromArgb(30, 30, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            panelMenu.MouseEnter += (s, e) => ExpandMenu();
            formPrincipal.Controls.Add(panelMenu);

            // Crear los botones
            btnClientes = CrearBoton("Clientes", Properties.Resources.client_icon, 10);
            btnProductos = CrearBoton("Productos", Properties.Resources.product_icon, 60);
            btnProveedor = CrearBoton("Proveedores", Properties.Resources.provider_icon, 110);
            btnVentas = CrearBoton("Ventas", Properties.Resources.sales_icon, 160);
            btnInventario = CrearBoton("Inventario", Properties.Resources.inventory_icon, 210);
            btnReportesCPP = CrearBoton("Reportes Generales", Properties.Resources.report_icon, 260);
            btnReportesVentas = CrearBoton("Reporte de Ventas", Properties.Resources.report_sales_icon, 310);
            btnReportesInventario = CrearBoton("Reporte de Inventario", Properties.Resources.report_inventory_icon, 360);
            btnAnalisisVentas = CrearBoton("Análisis de Ventas", Properties.Resources.analytics_icon, 410); // Nuevo botón
            btnSalir = CrearBoton("Salir", Properties.Resources.exit_icon, 460, true); // Ajustar posición

            // Añadir los botones al panel
            panelMenu.Controls.AddRange(new Control[]
            {
                btnClientes, btnProductos, btnProveedor, btnVentas, btnInventario,
                btnReportesCPP, btnReportesVentas, btnReportesInventario, btnAnalisisVentas, btnSalir
            });
        }

        private Button CrearBoton(string texto, Image icono, int posicionY, bool esSalir = false)
        {
            Button btn = new Button
            {
                Text = isMenuExpanded ? texto : "", // Solo muestra el texto si el menú está expandido
                Tag = texto, // Guardamos el texto en la propiedad Tag
                Image = icono,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Size = new Size(200, 40),
                Location = new Point(10, posicionY),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = esSalir ? Color.FromArgb(80, 0, 80) : Color.FromArgb(45, 45, 48),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Padding(10, 0, 0, 0)
            };

            btn.FlatAppearance.BorderSize = 0;

            // Efecto Hover animado
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = esSalir ? Color.FromArgb(110, 0, 110) : Color.FromArgb(75, 0, 130);
                btn.Font = new Font(btn.Font, FontStyle.Bold); // Negrita al pasar el cursor
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = esSalir ? Color.FromArgb(80, 0, 80) : Color.FromArgb(45, 45, 48);
                btn.Font = new Font(btn.Font, FontStyle.Regular);
            };

            return btn;
        }

        private void InitializeAnimation()
        {
            animationTimer = new Timer { Interval = 10 }; // Controla la velocidad de animación
            animationTimer.Tick += AnimateMenu;
            isMenuExpanded = false;
            targetWidth = collapsedWidth;
        }

        private void InitializeCollapseTimer()
        {
            collapseTimer = new Timer { Interval = 500 }; // Intervalo para verificar si el mouse está fuera
            collapseTimer.Tick += (s, e) =>
            {
                // Si el cursor está fuera del panel de menú, colapsarlo
                if (!panelMenu.Bounds.Contains(formPrincipal.PointToClient(Cursor.Position)))
                {
                    CollapseMenu();
                    collapseTimer.Stop();
                }
            };
        }

        private void AnimateMenu(object sender, EventArgs e)
        {
            int step = 10;
            if (panelMenu.Width != targetWidth)
            {
                if (panelMenu.Width < targetWidth)
                    panelMenu.Width = Math.Min(panelMenu.Width + step, targetWidth);
                else
                    panelMenu.Width = Math.Max(panelMenu.Width - step, targetWidth);

                panelMenu.Location = new Point(formPrincipal.ClientSize.Width - panelMenu.Width, 0); // Actualiza la posición para mantenerse en el borde derecho
            }
            else
            {
                animationTimer.Stop();
            }
        }

        private void ExpandMenu()
        {
            if (!isMenuExpanded)
            {
                isMenuExpanded = true;
                targetWidth = expandedWidth;
                animationTimer.Start();
                collapseTimer.Start(); // Inicia el temporizador de colapso cuando se expande el menú
                foreach (Button btn in panelMenu.Controls)
                {
                    btn.Text = btn.Tag.ToString(); // Muestra el texto guardado en Tag
                }
            }
        }

        private void CollapseMenu()
        {
            if (isMenuExpanded)
            {
                isMenuExpanded = false;
                targetWidth = collapsedWidth;
                animationTimer.Start();
                foreach (Button btn in panelMenu.Controls)
                {
                    btn.Text = ""; // Oculta el texto de cada botón
                }
            }
        }

        // Métodos para acceder a los botones y asignar eventos desde FormPrincipal
        public Button BtnClientes => btnClientes;
        public Button BtnProductos => btnProductos;
        public Button BtnProveedor => btnProveedor;
        public Button BtnVentas => btnVentas;
        public Button BtnInventario => btnInventario;
        public Button BtnReportesCPP => btnReportesCPP;
        public Button BtnReportesVentas => btnReportesVentas;
        public Button BtnReportesInventario => btnReportesInventario;
        public Button BtnAnalisisVentas => btnAnalisisVentas; // Propiedad del nuevo botón
        public Button BtnSalir => btnSalir;
    }
}
