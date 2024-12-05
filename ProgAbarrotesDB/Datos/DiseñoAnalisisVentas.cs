using ProgAbarrotesDB.Forms;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProgAbarrotesDB.Components
{
    public class DiseñoAnalisisVentas
    {
        private FormAnalisisVentas form;

        public TabControl TabControlCharts { get; private set; }

        // Gráficos
        public Chart ChartProductosVendidos { get; private set; }
        public Chart ChartVentasPorMes { get; private set; }
        public Chart ChartVentasPorCliente { get; private set; }
        public Chart ChartIngresosPorDia { get; private set; }

        // Controles para el rango de fechas
        public DateTimePicker DtpFechaInicio { get; private set; }
        public DateTimePicker DtpFechaFin { get; private set; }
        public Button BtnCargarIngresosPorDia { get; private set; }

        public DiseñoAnalisisVentas(FormAnalisisVentas form)
        {
            this.form = form;
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            // Configurar el TabControl
            TabControlCharts = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Crear TabPages
            TabPage tabProductosVendidos = new TabPage("Productos Vendidos");
            TabPage tabVentasPorMes = new TabPage("Ventas por Mes");
            TabPage tabVentasPorCliente = new TabPage("Ventas por Cliente");
            TabPage tabIngresosPorDia = new TabPage("Ingresos por Día");

            // Inicializar Charts
            ChartProductosVendidos = CrearChart();
            ChartVentasPorMes = CrearChart();
            ChartVentasPorCliente = CrearChart();
            ChartIngresosPorDia = CrearChart();

            // Agregar Charts a las TabPages
            tabProductosVendidos.Controls.Add(ChartProductosVendidos);
            tabVentasPorMes.Controls.Add(ChartVentasPorMes);
            tabVentasPorCliente.Controls.Add(ChartVentasPorCliente);

            // Para Ingresos por Día, utilizar un TableLayoutPanel para organizar los controles
            TableLayoutPanel panelIngresosPorDia = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                ColumnCount = 6,
                RowCount = 1,
                AutoSize = true
            };

            // Definir columnas con tamaños proporcionales
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F)); // Label Fecha Inicio
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F)); // DateTimePicker Fecha Inicio
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F)); // Label Fecha Fin
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F)); // DateTimePicker Fecha Fin
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F)); // Botón
            panelIngresosPorDia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // Espacio restante

            // Crear y agregar controles al panel
            Label lblFechaInicio = new Label { Text = "Fecha Inicio:", Anchor = AnchorStyles.Left, AutoSize = true };
            DtpFechaInicio = new DateTimePicker { Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = 140 };

            Label lblFechaFin = new Label { Text = "Fecha Fin:", Anchor = AnchorStyles.Left, AutoSize = true };
            DtpFechaFin = new DateTimePicker { Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = 140 };

            BtnCargarIngresosPorDia = new Button { Text = "Cargar Datos", Anchor = AnchorStyles.Left };

            // Añadir controles al panel con sus posiciones
            panelIngresosPorDia.Controls.Add(lblFechaInicio, 0, 0);
            panelIngresosPorDia.Controls.Add(DtpFechaInicio, 1, 0);
            panelIngresosPorDia.Controls.Add(lblFechaFin, 2, 0);
            panelIngresosPorDia.Controls.Add(DtpFechaFin, 3, 0);
            panelIngresosPorDia.Controls.Add(BtnCargarIngresosPorDia, 4, 0);

            // Añadir el panel y el chart a la TabPage
            tabIngresosPorDia.Controls.Add(panelIngresosPorDia);
            tabIngresosPorDia.Controls.Add(ChartIngresosPorDia);
            ChartIngresosPorDia.Dock = DockStyle.Fill;

            // Agregar TabPages al TabControl
            TabControlCharts.TabPages.AddRange(new TabPage[] { tabProductosVendidos, tabVentasPorMes, tabVentasPorCliente, tabIngresosPorDia });

            // Agregar el TabControl al formulario
            form.Controls.Add(TabControlCharts);
        }

        private Chart CrearChart()
        {
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chart.ChartAreas.Add(new ChartArea("MainArea"));
            return chart;
        }
    }
}
