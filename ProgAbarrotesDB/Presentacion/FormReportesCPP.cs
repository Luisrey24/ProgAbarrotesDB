using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB
{
    public partial class FormReportesCPP : Form
    {
        private ReportesGenerales diseñoReporte;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormReportesCPP(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext(); // Inicializar DataContext

            InitializeForm(); // Mover esta línea antes

            // Ahora podemos cargar los datos porque diseñoReporte ya está inicializado
            CargarClientes();
            CargarProveedores();
            CargarProductos();

            // Liberar DataContext al cerrar el formulario
            this.FormClosed += FormReportesCPP_FormClosed;
        }

        private void FormReportesCPP_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose(); // Liberar DataContext
        }

        private void InitializeForm()
        {
            // Inicializar y agregar el diseño de reporte
            diseñoReporte = new ReportesGenerales(this);

            // Asignar eventos a los ComboBoxes
            AsignarEventos();

            // Configurar los DataGridViews
            ConfigurarDataGridViews();
        }

        private void AsignarEventos()
        {
            diseñoReporte.CmbClientes.SelectedIndexChanged += CmbClientes_SelectedIndexChanged;
            diseñoReporte.CmbProveedores.SelectedIndexChanged += CmbProveedores_SelectedIndexChanged;
            diseñoReporte.CmbProductos.SelectedIndexChanged += CmbProductos_SelectedIndexChanged;
        }

        private void ConfigurarDataGridViews()
        {
            // Configurar DataGridView de Clientes
            diseñoReporte.DgvClientes.Columns.Clear();
            diseñoReporte.DgvClientes.Columns.Add("ID_Cliente", "ID");
            diseñoReporte.DgvClientes.Columns.Add("Nombre", "Nombre");
            diseñoReporte.DgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoReporte.DgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoReporte.DgvClientes.ReadOnly = true;

            // Configurar DataGridView de Proveedores
            diseñoReporte.DgvProveedores.Columns.Clear();
            diseñoReporte.DgvProveedores.Columns.Add("ID_Proveedor", "ID");
            diseñoReporte.DgvProveedores.Columns.Add("Nombre", "Nombre");
            diseñoReporte.DgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoReporte.DgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoReporte.DgvProveedores.ReadOnly = true;

            // Configurar DataGridView de Productos
            diseñoReporte.DgvProductos.Columns.Clear();
            diseñoReporte.DgvProductos.Columns.Add("ID_Producto", "ID");
            diseñoReporte.DgvProductos.Columns.Add("Nombre", "Nombre");
            diseñoReporte.DgvProductos.Columns.Add("Stock", "Stock");
            diseñoReporte.DgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoReporte.DgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoReporte.DgvProductos.ReadOnly = true;
        }

        private void CargarClientes()
        {
            try
            {
                var clientes = db.Cliente.Select(c => new { c.ID_Cliente, c.Nombre }).ToList();

                // Agregar opción para 'Todos'
                clientes.Insert(0, new { ID_Cliente = 0, Nombre = "Todos" });

                diseñoReporte.CmbClientes.DisplayMember = "Nombre";  // Muestra el nombre
                diseñoReporte.CmbClientes.ValueMember = "ID_Cliente";  // Usa el ID internamente
                diseñoReporte.CmbClientes.DataSource = clientes;
                diseñoReporte.CmbClientes.SelectedIndex = 0; // Seleccionar 'Todos'

                // Cargar DataGridView
                CargarDataGridViewClientes(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProveedores()
        {
            try
            {
                var proveedores = db.Proveedor.Select(p => new { p.ID_Proveedor, p.Nombre }).ToList();

                // Agregar opción para 'Todos'
                proveedores.Insert(0, new { ID_Proveedor = 0, Nombre = "Todos" });

                diseñoReporte.CmbProveedores.DisplayMember = "Nombre";  // Muestra el nombre
                diseñoReporte.CmbProveedores.ValueMember = "ID_Proveedor";  // Usa el ID internamente
                diseñoReporte.CmbProveedores.DataSource = proveedores;
                diseñoReporte.CmbProveedores.SelectedIndex = 0; // Seleccionar 'Todos'

                // Cargar DataGridView
                CargarDataGridViewProveedores(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductos()
        {
            try
            {
                var productos = db.Producto.Select(p => new { p.ID_Producto, p.Nombre }).ToList();

                // Agregar opción para 'Todos'
                productos.Insert(0, new { ID_Producto = 0, Nombre = "Todos" });

                diseñoReporte.CmbProductos.DisplayMember = "Nombre";  // Muestra el nombre
                diseñoReporte.CmbProductos.ValueMember = "ID_Producto";  // Usa el ID internamente
                diseñoReporte.CmbProductos.DataSource = productos;
                diseñoReporte.CmbProductos.SelectedIndex = 0; // Seleccionar 'Todos'

                // Cargar DataGridView
                CargarDataGridViewProductos(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idCliente = (int)diseñoReporte.CmbClientes.SelectedValue;
            CargarDataGridViewClientes(idCliente);
        }

        private void CmbProveedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idProveedor = (int)diseñoReporte.CmbProveedores.SelectedValue;
            CargarDataGridViewProveedores(idProveedor);
        }

        private void CmbProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idProducto = (int)diseñoReporte.CmbProductos.SelectedValue;
            CargarDataGridViewProductos(idProducto);
        }

        private void CargarDataGridViewClientes(int idCliente)
        {
            try
            {
                var query = db.Cliente.AsQueryable();

                if (idCliente != 0)
                {
                    query = query.Where(c => c.ID_Cliente == idCliente);
                }

                var clientes = query.Select(c => new
                {
                    c.ID_Cliente,
                    c.Nombre
                }).ToList();

                // Limpiar DataGridView
                diseñoReporte.DgvClientes.Rows.Clear();

                foreach (var cliente in clientes)
                {
                    diseñoReporte.DgvClientes.Rows.Add(cliente.ID_Cliente, cliente.Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDataGridViewProveedores(int idProveedor)
        {
            try
            {
                var query = db.Proveedor.AsQueryable();

                if (idProveedor != 0)
                {
                    query = query.Where(p => p.ID_Proveedor == idProveedor);
                }

                var proveedores = query.Select(p => new
                {
                    p.ID_Proveedor,
                    p.Nombre
                }).ToList();

                // Limpiar DataGridView
                diseñoReporte.DgvProveedores.Rows.Clear();

                foreach (var proveedor in proveedores)
                {
                    diseñoReporte.DgvProveedores.Rows.Add(proveedor.ID_Proveedor, proveedor.Nombre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDataGridViewProductos(int idProducto)
        {
            try
            {
                var query = db.Producto.AsQueryable();

                if (idProducto != 0)
                {
                    query = query.Where(p => p.ID_Producto == idProducto);
                }

                var productos = query.Select(p => new
                {
                    p.ID_Producto,
                    p.Nombre,
                    Stock = p.Stock > 0 ? p.Stock.ToString() : "Sin stock"
                }).ToList();

                // Limpiar DataGridView
                diseñoReporte.DgvProductos.Rows.Clear();

                foreach (var producto in productos)
                {
                    diseñoReporte.DgvProductos.Rows.Add(producto.ID_Producto, producto.Nombre, producto.Stock);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
