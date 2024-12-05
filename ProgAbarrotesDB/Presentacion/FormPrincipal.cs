using ProgAbarrotesDB.Clases; // Añadido
using ProgAbarrotesDB.Datos;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Forms
{
    public partial class FormPrincipal : Form
    {
        private DiseñoMenu diseñoMenu;
        private ClaseConexion conexion;

        public FormPrincipal(ClaseConexion conexionDb)
        {
            InitializeComponent();
            InitializeForm();
            conexion = conexionDb;
        }

        private void InitializeForm()
        {
            // Configurar el formulario principal
            this.Text = "PUNTO DE VENTA Ramses_comerciable";
            this.WindowState = FormWindowState.Maximized; // Iniciar maximizado
            this.StartPosition = FormStartPosition.CenterScreen;

            // Inicializar y agregar el menú
            diseñoMenu = new DiseñoMenu(this);

            // Asignar eventos a los botones del menú
            AsignarEventosMenu();

            // Configurar el área principal (Panel de contenido)
            Panel panelContenido = new Panel
            {
                Name = "panelContenido",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(panelContenido);
        }

        private void AsignarEventosMenu()
        {
            diseñoMenu.BtnClientes.Click += BtnClientes_Click;
            diseñoMenu.BtnProductos.Click += BtnProductos_Click;
            diseñoMenu.BtnProveedor.Click += BtnProveedor_Click;
            diseñoMenu.BtnVentas.Click += BtnVentas_Click;
            diseñoMenu.BtnInventario.Click += BtnInventario_Click;
            diseñoMenu.BtnReportesCPP.Click += BtnReportesCPP_Click;
            diseñoMenu.BtnReportesVentas.Click += BtnReportesVentas_Click;
            diseñoMenu.BtnReportesInventario.Click += BtnReportesInventario_Click;
            diseñoMenu.BtnAnalisisVentas.Click += BtnAnalisisVentas_Click; // Asignar evento al nuevo botón
            diseñoMenu.BtnSalir.Click += BtnSalir_Click;
        }

        // Métodos de evento para los botones del menú
        private void BtnClientes_Click(object sender, EventArgs e)
        {
                AbrirFormulario(new FormClientes(conexion));
        }

        private void BtnProductos_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormProductos(conexion));
        }

        private void BtnProveedor_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FormProveedor(conexion));
        }

        private void BtnVentas_Click(object sender, EventArgs e)
        {
              AbrirFormulario(new FormVentas(conexion));
        }

        private void BtnInventario_Click(object sender, EventArgs e)
        {
             AbrirFormulario(new FormInventario(conexion));
        }

        private void BtnReportesCPP_Click(object sender, EventArgs e)
        {
             AbrirFormulario(new FormReportesCPP(conexion));
        }

        private void BtnReportesVentas_Click(object sender, EventArgs e)
        {
              AbrirFormulario(new FormReportesVentas(conexion));
        }

        private void BtnReportesInventario_Click(object sender, EventArgs e)
        {
              AbrirFormulario(new FormReportesInventario(conexion));
        }

        private void BtnAnalisisVentas_Click(object sender, EventArgs e)
        {
            // Abrir el formulario de Análisis de Ventas
              AbrirFormulario(new FormAnalisisVentas(conexion));
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea salir?", "Confirmar Salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Método para abrir formularios dentro del panel de contenido
        private void AbrirFormulario(Form formulario)
        {
            Panel panelContenido = this.Controls["panelContenido"] as Panel;
            if (panelContenido != null)
            {
                panelContenido.Controls.Clear();
                formulario.TopLevel = false;
                formulario.FormBorderStyle = FormBorderStyle.None;
                formulario.Dock = DockStyle.Fill;
                panelContenido.Controls.Add(formulario);
                formulario.Show();
            }
        }
    }
}
