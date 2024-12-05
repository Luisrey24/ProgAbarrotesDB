using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB
{
    public partial class FormReportesVentas : Form
    {
        private DiseñoReporteVentas diseñoReporteVentas;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormReportesVentas(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            InitializeForm();

            // Liberar DataContext al cerrar el formulario
            this.FormClosed += FormReportesVentas_FormClosed;
        }

        private void InitializeForm()
        {
            db = conexion.GetDataContext();
            diseñoReporteVentas = new DiseñoReporteVentas(this);
            CargarClientes(); // Cargar clientes en el ComboBox

            AsignarEventos();
            ConfigurarDataGridView();

            diseñoReporteVentas.BtnGenerarReporte.Enabled = false;
            diseñoReporteVentas.CmbCliente.SelectedIndexChanged += HabilitarBotonGenerarReporte;
            diseñoReporteVentas.DtpFecha.ValueChanged += HabilitarBotonGenerarReporte;
        }

        private void FormReportesVentas_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose(); // Liberar DataContext
        }

        private void AsignarEventos()
        {
            diseñoReporteVentas.BtnGenerarReporte.Click += BtnGenerarReporte_Click;
        }

        private void HabilitarBotonGenerarReporte(object sender, EventArgs e)
        {
            diseñoReporteVentas.BtnGenerarReporte.Enabled =
                diseñoReporteVentas.CmbCliente.SelectedItem != null &&
                diseñoReporteVentas.DtpFecha.Value.Date != DateTime.MinValue;
        }

        private void CargarClientes()
        {
            try
            {
                var clientes = (from c in db.Cliente
                                select new { c.ID_Cliente, c.Nombre }).ToList();

                diseñoReporteVentas.CmbCliente.DisplayMember = "Nombre";
                diseñoReporteVentas.CmbCliente.ValueMember = "ID_Cliente";
                diseñoReporteVentas.CmbCliente.DataSource = clientes;

                if (diseñoReporteVentas.CmbCliente.Items.Count > 0)
                    diseñoReporteVentas.CmbCliente.SelectedIndex = 0;
                else
                    diseñoReporteVentas.CmbCliente.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGenerarReporte_Click(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = diseñoReporteVentas.DtpFecha.Value.Date;
            int idClienteSeleccionado = (int)diseñoReporteVentas.CmbCliente.SelectedValue;

            try
            {
                var query = from v in db.Venta
                            join dv in db.Detalle_Venta on v.ID_Venta equals dv.ID_Venta
                            join p in db.Producto on dv.ID_Producto equals p.ID_Producto
                            where v.ID_Cliente == idClienteSeleccionado &&
                                  v.Fecha.Date == fechaSeleccionada
                            select new
                            {
                                Cliente = v.Cliente.Nombre,
                                Producto = p.Nombre,
                                Cantidad = dv.Cantidad,
                                Total = (decimal?)dv.Subtotal, // Aseguramos que sea nullable
                                Fecha = (DateTime?)v.Fecha // Aseguramos que sea nullable
                            };

                diseñoReporteVentas.DgvReporteVentas.Rows.Clear();

                foreach (var item in query)
                {
                    string totalFormateado = item.Total.HasValue ? item.Total.Value.ToString("C") : "N/A";
                    string fechaFormateada = item.Fecha.HasValue ? item.Fecha.Value.ToShortDateString() : "N/A";

                    diseñoReporteVentas.DgvReporteVentas.Rows.Add(
                        item.Cliente,
                        item.Producto,
                        item.Cantidad,
                        totalFormateado,
                        fechaFormateada
                    );
                }

                if (diseñoReporteVentas.DgvReporteVentas.Rows.Count > 0)
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

        private void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627:
                    MessageBox.Show("Error: Un registro duplicado está causando un conflicto.", "Error de Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 547:
                    MessageBox.Show("Error: Restricción de integridad referencial violada.", "Restricción de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 2601:
                    MessageBox.Show("Error: Índice único duplicado detectado.", "Duplicación de Índice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 229:
                    MessageBox.Show("No tienes permisos suficientes para ejecutar esta operación.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Ocurrió un error de base de datos: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void ConfigurarDataGridView()
        {
            diseñoReporteVentas.DgvReporteVentas.Columns.Clear();

            diseñoReporteVentas.DgvReporteVentas.Columns.Add("Cliente", "Cliente");
            diseñoReporteVentas.DgvReporteVentas.Columns.Add("Producto", "Producto");
            diseñoReporteVentas.DgvReporteVentas.Columns.Add("Cantidad", "Cantidad");
            diseñoReporteVentas.DgvReporteVentas.Columns.Add("Total", "Total");
            diseñoReporteVentas.DgvReporteVentas.Columns.Add("Fecha", "Fecha");

            diseñoReporteVentas.DgvReporteVentas.Columns["Total"].DefaultCellStyle.Format = "C2";
            diseñoReporteVentas.DgvReporteVentas.Columns["Fecha"].DefaultCellStyle.Format = "d";

            diseñoReporteVentas.DgvReporteVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoReporteVentas.DgvReporteVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoReporteVentas.DgvReporteVentas.MultiSelect = false;
            diseñoReporteVentas.DgvReporteVentas.ReadOnly = true;
            diseñoReporteVentas.DgvReporteVentas.AllowUserToAddRows = false;
            diseñoReporteVentas.DgvReporteVentas.AllowUserToDeleteRows = false;
            diseñoReporteVentas.DgvReporteVentas.AllowUserToOrderColumns = true;
        }
    }
}
