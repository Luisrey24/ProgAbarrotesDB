using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace ProgAbarrotesDB
{
    public partial class FormReportesInventario : Form
    {
        private DiseñoReporteInventario diseñoReporteInventario;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormReportesInventario(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext(); // Inicializar DataContext
            InitializeForm();

            // Liberar DataContext al cerrar el formulario
            this.FormClosed += FormReportesInventario_FormClosed;
        }

        private void FormReportesInventario_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose(); // Liberar el DataContext
        }

        private void InitializeForm()
        {
            // Inicializar y agregar el diseño de reporte de inventario
            diseñoReporteInventario = new DiseñoReporteInventario(this);

            // Cargar proveedores en el ComboBox
            CargarProveedores();

            // Asignar eventos a los botones
            AsignarEventos();

            // Configurar el DataGridView con las columnas necesarias
            ConfigurarDataGridView();

            // Deshabilitar botón Generar Reporte hasta que se seleccione un proveedor y fecha
            diseñoReporteInventario.BtnGenerarReporte.Enabled = false;
            diseñoReporteInventario.CmbProveedor.SelectedIndexChanged += HabilitarBotonGenerarReporte;
            diseñoReporteInventario.DtpFecha.ValueChanged += HabilitarBotonGenerarReporte;
        }

        private void HabilitarBotonGenerarReporte(object sender, EventArgs e)
        {
            diseñoReporteInventario.BtnGenerarReporte.Enabled =
                diseñoReporteInventario.CmbProveedor.SelectedItem != null &&
                diseñoReporteInventario.DtpFecha.Value.Date != DateTime.MinValue;
        }

        private void CargarProveedores()
        {
            try
            {
                var proveedores = (from p in db.Proveedor
                                   select new { p.ID_Proveedor, p.Nombre }).ToList();

                diseñoReporteInventario.CmbProveedor.DisplayMember = "Nombre";
                diseñoReporteInventario.CmbProveedor.ValueMember = "ID_Proveedor";
                diseñoReporteInventario.CmbProveedor.DataSource = proveedores;

                if (diseñoReporteInventario.CmbProveedor.Items.Count > 0)
                    diseñoReporteInventario.CmbProveedor.SelectedIndex = 0;
                else
                    diseñoReporteInventario.CmbProveedor.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarEventos()
        {
            diseñoReporteInventario.BtnGenerarReporte.Click += BtnGenerarReporte_Click;
        }

        // Evento para el botón Generar Reporte
        private void BtnGenerarReporte_Click(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = diseñoReporteInventario.DtpFecha.Value.Date;
            int idProveedorSeleccionado = (int)diseñoReporteInventario.CmbProveedor.SelectedValue;

            try
            {
                var query = from inv in db.Inventarios
                            join p in db.Producto on inv.ID_Producto equals p.ID_Producto
                            where inv.ID_Proveedor == idProveedorSeleccionado &&
                                  inv.FechaEntrada.HasValue &&
                                  inv.FechaEntrada.Value.Date == fechaSeleccionada
                            select new
                            {
                                Proveedor = inv.Proveedor.Nombre,
                                Producto = p.Nombre,
                                Cantidad = inv.CantidadEntrada,
                                CostoUnitario = inv.PrecioPorUnidad,
                                Total = inv.Total,
                                Fecha = inv.FechaEntrada,
                                Observaciones = inv.Observaciones
                            };

                // Limpiar el DataGridView antes de agregar nuevos datos
                diseñoReporteInventario.DgvReporteInventario.Rows.Clear();

                foreach (var item in query)
                {
                    string fechaFormateada = item.Fecha.HasValue ? item.Fecha.Value.ToShortDateString() : "N/A";

                    string costoUnitarioFormateado = item.CostoUnitario.HasValue
                        ? item.CostoUnitario.Value.ToString("C2")
                        : "N/A";

                    string totalFormateado = item.Total.HasValue
                        ? item.Total.Value.ToString("C2")
                        : "N/A";

                    string observaciones = item.Observaciones ?? "";

                    diseñoReporteInventario.DgvReporteInventario.Rows.Add(
                        item.Proveedor,
                        item.Producto,
                        item.Cantidad,
                        costoUnitarioFormateado,
                        totalFormateado,
                        fechaFormateada,
                        observaciones
                    );
                }

                // Verificar si se agregaron filas
                if (diseñoReporteInventario.DgvReporteInventario.Rows.Count > 0)
                {
                    MessageBox.Show("Reporte generado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontraron registros que coincidan con los filtros ingresados.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al generar el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para manejar excepciones SQL específicas
        private void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627: // Clave duplicada
                    MessageBox.Show("Ya existe un registro con los mismos datos únicos.", "Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 547: // Restricción de clave foránea
                    MessageBox.Show("No se puede realizar la acción debido a restricciones de integridad referencial.", "Restricción de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 2601: // Índice único duplicado
                    MessageBox.Show("Un valor único ya existe en el sistema.", "Índice Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 229: // Permisos insuficientes
                    MessageBox.Show("No tienes permisos para realizar esta operación.", "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Error de base de datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // Método para configurar el DataGridView con columnas específicas
        private void ConfigurarDataGridView()
        {
            diseñoReporteInventario.DgvReporteInventario.Columns.Clear();

            // Definir las columnas del DataGridView
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Proveedor", "Proveedor");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Producto", "Producto");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Cantidad", "Cantidad");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("CostoUnitario", "Costo Unitario");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Total", "Total");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Fecha", "Fecha");
            diseñoReporteInventario.DgvReporteInventario.Columns.Add("Observaciones", "Observaciones");

            // Configurar el formato de las columnas "CostoUnitario" y "Total"
            diseñoReporteInventario.DgvReporteInventario.Columns["CostoUnitario"].DefaultCellStyle.Format = "C2"; // Formato de moneda
            diseñoReporteInventario.DgvReporteInventario.Columns["Total"].DefaultCellStyle.Format = "C2"; // Formato de moneda
            diseñoReporteInventario.DgvReporteInventario.Columns["Fecha"].DefaultCellStyle.Format = "d"; // Formato de fecha corta

            // Ajustar configuración del DataGridView
            diseñoReporteInventario.DgvReporteInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoReporteInventario.DgvReporteInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoReporteInventario.DgvReporteInventario.MultiSelect = false;
            diseñoReporteInventario.DgvReporteInventario.ReadOnly = true;
            diseñoReporteInventario.DgvReporteInventario.AllowUserToAddRows = false;
            diseñoReporteInventario.DgvReporteInventario.AllowUserToDeleteRows = false;
            diseñoReporteInventario.DgvReporteInventario.AllowUserToOrderColumns = true;
        }
    }
}
