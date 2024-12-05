using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB.Forms
{
    public partial class FormAnalisisVentas : Form
    {
        private ClaseConexion conexion;
        private DataClasses1DataContext db;
        private DiseñoAnalisisVentas diseñoAnalisisVentas;

        public FormAnalisisVentas(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext();

            InitializeForm();
        }

        private void InitializeForm()
        {
            diseñoAnalisisVentas = new DiseñoAnalisisVentas(this);

            // Asignar eventos
            AsignarEventos();

            // Cargar datos en los gráficos
            CargarProductosVendidos();
            CargarVentasPorMes();
            CargarVentasPorCliente();

            // Liberar DataContext al cerrar el formulario
            this.FormClosed += FormAnalisisVentas_FormClosed;
        }

        private void FormAnalisisVentas_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose();
        }

        private void AsignarEventos()
        {
            diseñoAnalisisVentas.BtnCargarIngresosPorDia.Click += BtnCargarIngresosPorDia_Click;
        }

        private void BtnCargarIngresosPorDia_Click(object sender, EventArgs e)
        {
            DateTime fechaInicio = diseñoAnalisisVentas.DtpFechaInicio.Value.Date;
            DateTime fechaFin = diseñoAnalisisVentas.DtpFechaFin.Value.Date;

            if (fechaInicio > fechaFin)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha de fin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarIngresosPorDia(fechaInicio, fechaFin);
        }

        private void CargarProductosVendidos()
        {
            try
            {
                var totalProductosVendidos = from dv in db.Detalle_Venta
                                             group dv by dv.Producto.Nombre into grupo
                                             select new
                                             {
                                                 Producto = grupo.Key,
                                                 TotalVendido = grupo.Sum(dv => dv.Cantidad)
                                             };

                var chart = diseñoAnalisisVentas.ChartProductosVendidos;
                chart.Series.Clear();
                Series series = new Series("Total Vendido")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true
                };
                chart.Series.Add(series);

                foreach (var item in totalProductosVendidos)
                {
                    series.Points.AddXY(item.Producto, item.TotalVendido);
                }

                chart.ChartAreas["MainArea"].AxisX.Title = "Producto";
                chart.ChartAreas["MainArea"].AxisY.Title = "Cantidad Vendida";
                chart.Titles.Clear();
                chart.Titles.Add("Total de Productos Vendidos");
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar datos del gráfico de productos vendidos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del gráfico de productos vendidos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentasPorMes()
        {
            try
            {
                var ventasPorMes = from v in db.Venta
                                   group v by new { v.Fecha.Year, v.Fecha.Month } into grupo
                                   orderby grupo.Key.Year, grupo.Key.Month
                                   select new
                                   {
                                       Fecha = new DateTime(grupo.Key.Year, grupo.Key.Month, 1),
                                       TotalVentas = grupo.Sum(v => v.Total)
                                   };

                var chart = diseñoAnalisisVentas.ChartVentasPorMes;
                chart.Series.Clear();
                Series series = new Series("Ventas Totales")
                {
                    ChartType = SeriesChartType.Line,
                    IsValueShownAsLabel = true
                };
                chart.Series.Add(series);

                foreach (var item in ventasPorMes)
                {
                    series.Points.AddXY(item.Fecha.ToString("MMM yyyy"), item.TotalVentas);
                }

                chart.ChartAreas["MainArea"].AxisX.Title = "Mes";
                chart.ChartAreas["MainArea"].AxisY.Title = "Total de Ventas";
                chart.ChartAreas["MainArea"].AxisX.Interval = 1;
                chart.ChartAreas["MainArea"].AxisX.LabelStyle.Angle = -45;
                chart.Titles.Clear();
                chart.Titles.Add("Ventas Totales por Mes");
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar datos del gráfico de ventas por mes.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del gráfico de ventas por mes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarVentasPorCliente()
        {
            try
            {
                var ventasPorCliente = from v in db.Venta
                                       group v by v.Cliente.Nombre into grupo
                                       select new
                                       {
                                           Cliente = grupo.Key,
                                           TotalVentas = grupo.Sum(v => v.Total)
                                       };

                var chart = diseñoAnalisisVentas.ChartVentasPorCliente;
                chart.Series.Clear();
                Series series = new Series("Ventas Totales")
                {
                    ChartType = SeriesChartType.Bar,
                    IsValueShownAsLabel = true
                };
                chart.Series.Add(series);

                foreach (var item in ventasPorCliente)
                {
                    series.Points.AddXY(item.Cliente, item.TotalVentas);
                }

                chart.ChartAreas["MainArea"].AxisX.Title = "Cliente";
                chart.ChartAreas["MainArea"].AxisY.Title = "Total de Ventas";
                chart.ChartAreas["MainArea"].AxisX.Interval = 1;
                chart.ChartAreas["MainArea"].AxisX.LabelStyle.Angle = -45;
                chart.Titles.Clear();
                chart.Titles.Add("Total de Ventas por Cliente");
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar datos del gráfico de ventas por cliente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del gráfico de ventas por cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarIngresosPorDia(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var ingresosPorDia = from v in db.Venta
                                     where v.Fecha.Date >= fechaInicio.Date && v.Fecha.Date <= fechaFin.Date
                                     group v by v.Fecha.Date into grupo
                                     orderby grupo.Key
                                     select new
                                     {
                                         Fecha = grupo.Key,
                                         TotalIngresos = grupo.Sum(v => v.Total)
                                     };

                var chart = diseñoAnalisisVentas.ChartIngresosPorDia;
                chart.Series.Clear();
                Series series = new Series("Ingresos")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true
                };
                chart.Series.Add(series);

                foreach (var item in ingresosPorDia)
                {
                    series.Points.AddXY(item.Fecha.ToShortDateString(), item.TotalIngresos);
                }

                chart.ChartAreas["MainArea"].AxisX.Title = "Fecha";
                chart.ChartAreas["MainArea"].AxisY.Title = "Ingresos";
                chart.ChartAreas["MainArea"].AxisX.Interval = 1;
                chart.ChartAreas["MainArea"].AxisX.LabelStyle.Angle = -45;
                chart.Titles.Clear();
                chart.Titles.Add($"Ingresos por Día ({fechaInicio.ToShortDateString()} - {fechaFin.ToShortDateString()})");
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar datos del gráfico de ingresos por día.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del gráfico de ingresos por día: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleSqlException(SqlException ex, string customMessage)
        {
            switch (ex.Number)
            {
                case 2627:
                    MessageBox.Show($"{customMessage}: Un registro duplicado está causando un conflicto.", "Error de Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 547:
                    MessageBox.Show($"{customMessage}: Restricción de integridad referencial violada.", "Restricción de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 2601:
                    MessageBox.Show($"{customMessage}: Índice único duplicado detectado.", "Duplicación de Índice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 229:
                    MessageBox.Show($"{customMessage}: No tienes permisos suficientes para ejecutar esta operación.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"{customMessage}: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
    }
}
