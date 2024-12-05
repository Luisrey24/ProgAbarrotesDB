using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.Windows.Forms;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB
{
    public partial class FormVentas : Form
    {
        private DiseñoVentas diseñoVentas;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;
        private decimal subtotal = 0m; // Variable para almacenar el subtotal

        public FormVentas(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext(); // Crear instancia de DataContext para toda la duración del formulario

            diseñoVentas = new DiseñoVentas(this);
            CargarClientes();
            CargarProductos();
            CargarMetodosPago();
            AsignarEventos();

            // Liberar el DataContext cuando se cierra el formulario
            this.FormClosed += (s, e) => db?.Dispose();
        }

        private void CargarClientes()
        {
            try
            {
                var clientes = (from c in db.Cliente
                                select new { c.ID_Cliente, c.Nombre }).ToList();

                diseñoVentas.CmbCliente.DisplayMember = "Nombre";
                diseñoVentas.CmbCliente.ValueMember = "ID_Cliente";
                diseñoVentas.CmbCliente.DataSource = clientes;

                if (diseñoVentas.CmbCliente.Items.Count > 0)
                    diseñoVentas.CmbCliente.SelectedIndex = 0;
                else
                    diseñoVentas.CmbCliente.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar los clientes.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductos()
        {
            try
            {
                var productos = from p in db.Producto
                                select p.Nombre;

                diseñoVentas.CmbProducto.Items.Clear();
                foreach (var producto in productos)
                {
                    diseñoVentas.CmbProducto.Items.Add(producto);
                }

                diseñoVentas.CmbProducto.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar los productos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarMetodosPago()
        {
            try
            {
                var metodosPago = from m in db.Metodo_Pago
                                  select m.Nombre;

                diseñoVentas.CmbMetodoPago.Items.Clear();
                foreach (var metodo in metodosPago)
                {
                    diseñoVentas.CmbMetodoPago.Items.Add(metodo);
                }

                if (diseñoVentas.CmbMetodoPago.Items.Count > 0)
                    diseñoVentas.CmbMetodoPago.SelectedIndex = 0;
                else
                    diseñoVentas.CmbMetodoPago.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al cargar los métodos de pago.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los métodos de pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsignarEventos()
        {
            diseñoVentas.BtnAgregar.Click += BtnAgregar_Click;
            diseñoVentas.BtnConsultar.Click += BtnConsultar_Click;
            diseñoVentas.BtnRegistrar.Click += BtnRegistrar_Click;
            diseñoVentas.BtnEliminar.Click += BtnEliminar_Click;
            diseñoVentas.CmbProducto.SelectedIndexChanged += CmbProducto_SelectedIndexChanged;

            // Evento para recalcular subtotal y total al eliminar una fila
            diseñoVentas.DgvVentas.RowsRemoved += DgvVentas_RowsRemoved;
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (diseñoVentas.DgvVentas.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = diseñoVentas.DgvVentas.SelectedRows[0];

                DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este producto?",
                                                            "Confirmar Eliminación",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question);
                if (confirmacion == DialogResult.Yes)
                {
                    diseñoVentas.DgvVentas.Rows.Remove(filaSeleccionada);
                    MessageBox.Show("Producto eliminado de la venta.", "Producto Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Recalcular subtotal
                    RecalcularSubtotal();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.", "Seleccione un Producto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            if (diseñoVentas.DgvVentas.Rows.Count == 0)
            {
                MessageBox.Show("No hay ventas para registrar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Validar stock antes de registrar
            foreach (DataGridViewRow fila in diseñoVentas.DgvVentas.Rows)
            {
                if (fila.IsNewRow) continue;

                string productoNombre = fila.Cells["Producto"].Value.ToString();
                int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);

                var producto = db.Producto.SingleOrDefault(p => p.Nombre == productoNombre);
                if (producto != null && producto.Stock < cantidad)
                {
                    MessageBox.Show($"No hay suficiente stock para el producto '{productoNombre}'.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Cancelar el registro de la venta
                }
            }

            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea registrar esta venta?", "Confirmar Registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string connectionString = conexion.GetDataContext().Connection.ConnectionString;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Insertar la venta y obtener el ID_Venta generado
                        int idVenta = 0;
                        using (SqlCommand cmdVenta = new SqlCommand("sp_InsertVenta", conn))
                        {
                            cmdVenta.CommandType = CommandType.StoredProcedure;
                            cmdVenta.Parameters.AddWithValue("@Fecha", DateTime.Now);
                            cmdVenta.Parameters.AddWithValue("@Importe", subtotal);
                            cmdVenta.Parameters.AddWithValue("@IVA", Math.Round(subtotal * 0.16m, 2));
                            cmdVenta.Parameters.AddWithValue("@Total", Math.Round(subtotal * 1.16m, 2));
                            cmdVenta.Parameters.AddWithValue("@Metodo_Pago", diseñoVentas.CmbMetodoPago.SelectedItem?.ToString() ?? "Por Definir");
                            cmdVenta.Parameters.AddWithValue("@ID_Cliente", (int)diseñoVentas.CmbCliente.SelectedValue);

                            SqlParameter outputId = new SqlParameter("@ID_Venta", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmdVenta.Parameters.Add(outputId);

                            cmdVenta.ExecuteNonQuery();

                            if (outputId.Value != DBNull.Value)
                            {
                                idVenta = (int)outputId.Value;
                            }
                            else
                            {
                                throw new Exception("No se pudo obtener el ID de la venta.");
                            }
                        }

                        // Insertar detalles de venta
                        foreach (DataGridViewRow fila in diseñoVentas.DgvVentas.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string productoNombre = fila.Cells["Producto"].Value.ToString();
                            int cantidad = Convert.ToInt32(fila.Cells["Cantidad"].Value);
                            decimal precioUnitario = Convert.ToDecimal(fila.Cells["PrecioUnitario"].Value.ToString().Replace("$", ""));
                            decimal total = Convert.ToDecimal(fila.Cells["Total"].Value.ToString().Replace("$", ""));

                            // Obtener el ID_Producto basado en el nombre
                            var producto = db.Producto.SingleOrDefault(p => p.Nombre == productoNombre);
                            if (producto == null)
                            {
                                MessageBox.Show($"Producto '{productoNombre}' no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                throw new Exception($"Producto '{productoNombre}' no encontrado.");
                            }

                            // Insertar el detalle de la venta
                            using (SqlCommand cmdDetalle = new SqlCommand("sp_InsertDetalleVenta", conn))
                            {
                                cmdDetalle.CommandType = CommandType.StoredProcedure;
                                cmdDetalle.Parameters.AddWithValue("@ID_Venta", idVenta);
                                cmdDetalle.Parameters.AddWithValue("@ID_Producto", producto.ID_Producto);
                                cmdDetalle.Parameters.AddWithValue("@Cantidad", cantidad);
                                cmdDetalle.Parameters.AddWithValue("@Precio_Unitario", precioUnitario);
                                // No incluir @Subtotal porque es calculada

                                cmdDetalle.ExecuteNonQuery();
                            }

                            // Nota: La actualización de Stock está manejada por el trigger trg_UpdateStockOnVentaDetalleInsert
                        }
                    }

                    scope.Complete(); // Confirmar la transacción
                }

                MessageBox.Show("Venta registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                diseñoVentas.DgvVentas.Rows.Clear();

                // Resetear subtotal y etiquetas
                subtotal = 0m;
                diseñoVentas.LblSubTotalValue.Text = "$0.00";
                diseñoVentas.LblIvaValue.Text = "$0.00";
                diseñoVentas.LblTotalValue.Text = "$0.00";
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al registrar la venta.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar la venta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            string productoSeleccionado = diseñoVentas.CmbProducto.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(productoSeleccionado))
            {
                decimal precio = ObtenerPrecioPorProducto(productoSeleccionado);
                diseñoVentas.LblCostoUnidad.Text = $"${precio}";
            }
            else
            {
                diseñoVentas.LblCostoUnidad.Text = "$";
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (diseñoVentas.CmbCliente.Items.Count == 0 || diseñoVentas.CmbCliente.SelectedValue == null)
                {
                    MessageBox.Show("No hay clientes disponibles para seleccionar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int idCliente = (int)diseñoVentas.CmbCliente.SelectedValue;
                string clienteNombre = diseñoVentas.CmbCliente.Text;
                string producto = diseñoVentas.CmbProducto.SelectedItem?.ToString();
                string cantidadText = diseñoVentas.TxtCantidad.Text.Trim();
                string metodoPago = diseñoVentas.CmbMetodoPago.SelectedItem?.ToString();
                DateTime fechaCompra = diseñoVentas.DtpFechaCompra.Value;

                if (string.IsNullOrEmpty(clienteNombre) || string.IsNullOrEmpty(producto) ||
                    string.IsNullOrEmpty(cantidadText) || string.IsNullOrEmpty(metodoPago))
                {
                    MessageBox.Show("Por favor, complete todos los campos necesarios para agregar un producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(cantidadText, out int cantidad) || cantidad <= 0)
                {
                    MessageBox.Show("Por favor, ingrese una cantidad válida (entero positivo).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal precioPorUnidad = ObtenerPrecioPorProducto(producto);

                if (precioPorUnidad <= 0)
                {
                    MessageBox.Show("El precio por unidad no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal totalProducto = precioPorUnidad * cantidad;

                // Agregar la fila al DataGridView
                diseñoVentas.DgvVentas.Rows.Add(
                    clienteNombre,
                    fechaCompra.ToShortDateString(),
                    producto,
                    cantidad,
                    precioPorUnidad.ToString("C"),
                    totalProducto.ToString("C"),
                    metodoPago,
                    idCliente
                );

                // Actualizar subtotal, IVA y total
                subtotal += totalProducto;
                ActualizarTotales();

                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex, "Error al agregar el producto.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al intentar agregar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarTotales()
        {
            decimal iva = Math.Round(subtotal * 0.16m, 2);
            decimal totalConIva = subtotal + iva;

            diseñoVentas.LblSubTotalValue.Text = subtotal.ToString("C");
            diseñoVentas.LblIvaValue.Text = iva.ToString("C");
            diseñoVentas.LblTotalValue.Text = totalConIva.ToString("C");
        }

        private void DgvVentas_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RecalcularSubtotal();
        }

        private void RecalcularSubtotal()
        {
            subtotal = diseñoVentas.DgvVentas.Rows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .Sum(row => Convert.ToDecimal(row.Cells["Total"].Value.ToString().Replace("$", "")));

            ActualizarTotales();
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
                    MessageBox.Show($"{customMessage}: No tienes permisos suficientes para ejecutar esta operación.", "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"{customMessage}: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private decimal ObtenerPrecioPorProducto(string producto)
        {
            try
            {
                var prod = db.Producto.SingleOrDefault(p => p.Nombre == producto);
                return prod?.Precio ?? 0.00m;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el precio del producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0.00m;
            }
        }

        private void LimpiarCampos()
        {
            diseñoVentas.CmbProducto.SelectedIndex = -1;
            diseñoVentas.TxtCantidad.Clear();
            diseñoVentas.LblCostoUnidad.Text = "$";
        }

        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = diseñoVentas.DtpFechaCompra.Value.Date;

            try
            {
                // Usar el DataContext existente (db)
                var ventas = from v in db.Venta
                             where v.Fecha.Date == fechaSeleccionada
                             select new
                             {
                                 v.ID_Venta,
                                 Cliente = v.Cliente.Nombre,
                                 Fecha = v.Fecha,
                                 Metodo_Pago = v.Metodo_Pago,
                                 Total = v.Total
                             };

                var listaVentas = ventas.ToList();

                if (listaVentas.Any())
                {
                    string mensaje = "Ventas encontradass:\n";
                    foreach (var venta in listaVentas)
                    {
                        mensaje += $"ID Venta: {venta.ID_Venta}, Cliente: {venta.Cliente}, Fecha: {venta.Fecha}, Método de Pago: {venta.Metodo_Pago}, Total: {venta.Total:C}\n";
                    }
                    MessageBox.Show(mensaje, "Ventas Encontradas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontraron ventas en la fecha seleccionada.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                MessageBox.Show("No tienes permisos para realizar consultas de ventas.", "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la consulta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
