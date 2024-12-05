using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Windows.Forms;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB
{
    public partial class FormInventario : Form
    {
        private DiseñoInventario diseñoInventario;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormInventario(ClaseConexion conexionDb)
        {
            InitializeComponent();
            this.conexion = conexionDb;
            this.db = conexion.GetDataContext(); // Crear instancia de DataContext al inicio
            InitializeForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose(); // Liberar el DataContext si está en uso
            }
            base.Dispose(disposing);
        }

        private void InitializeForm()
        {
            diseñoInventario = new DiseñoInventario(this);
            CargarProductos();
            CargarProveedores();
            AsignarEventos();
            ConfigurarDataGridView();
            ActualizarTotales(); // Inicializar los totales en 0
        }

        private void AsignarEventos()
        {
            diseñoInventario.BtnAgregar.Click += BtnAgregar_Click;
            diseñoInventario.BtnAñadirInventario.Click += BtnAñadirInventario_Click;
            diseñoInventario.CmbProducto.SelectedIndexChanged += CmbProducto_SelectedIndexChanged;
            diseñoInventario.BtnLimpiar.Click += BtnLimpiar_Click;
            diseñoInventario.DgvInventario.RowsRemoved += DgvInventario_RowsRemoved;
            //diseñoInventario.BtnConsultar.Click += BtnConsultar_Click; // Asignar evento
            //diseñoInventario.BtnEliminar.Click += BtnEliminar_Click;   // Asignar evento
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            string producto = diseñoInventario.CmbProducto.SelectedItem?.ToString();
            string proveedor = diseñoInventario.CmbProveedor.SelectedItem?.ToString();
            // DateTime fechaEntrada = diseñoInventario.DtpFecha.Value; // No utilizado en el procedimiento almacenado
            string observaciones = diseñoInventario.TxtObservaciones.Text.Trim();
            string cantidadEntradaText = diseñoInventario.TxtCantidadEntrada.Text.Trim();

            if (string.IsNullOrEmpty(producto) || string.IsNullOrEmpty(proveedor) || string.IsNullOrEmpty(cantidadEntradaText))
            {
                MessageBox.Show("Por favor, complete los campos de Producto, Proveedor y Cantidad Entrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(cantidadEntradaText, out int cantidadEntrada) || cantidadEntrada <= 0)
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida (entero positivo).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal precioPorUnidad = ObtenerCostoPorProducto(producto);

            if (precioPorUnidad <= 0)
            {
                MessageBox.Show("El costo por unidad no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal total = precioPorUnidad * cantidadEntrada;

            diseñoInventario.DgvInventario.Rows.Add(
                producto,
                proveedor,
                DateTime.Now.ToShortDateString(), // FechaEntrada es gestionada por el procedimiento almacenado
                cantidadEntrada,
                precioPorUnidad.ToString("C2"),
                total.ToString("C2"),
                observaciones
            );

            ActualizarTotales();
            LimpiarCampos();
        }

        private void BtnAñadirInventario_Click(object sender, EventArgs e)
        {
            if (diseñoInventario.DgvInventario.Rows.Count == 0)
            {
                MessageBox.Show("No hay entradas para añadir al inventario.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea añadir estas entradas al inventario?", "Confirmar Añadir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

                        foreach (DataGridViewRow fila in diseñoInventario.DgvInventario.Rows)
                        {
                            if (fila.IsNewRow) continue;

                            string productoNombre = fila.Cells["Producto"].Value.ToString();
                            string proveedorNombre = fila.Cells["Proveedor"].Value.ToString();
                            // DateTime fechaEntrada = DateTime.Parse(fila.Cells["Fecha"].Value.ToString()); // No utilizado
                            int cantidadEntrada = Convert.ToInt32(fila.Cells["Cantidad Entrada"].Value);
                            decimal precioPorUnidad = decimal.Parse(fila.Cells["Precio por Unidad"].Value.ToString(), System.Globalization.NumberStyles.Currency);
                            // decimal total = decimal.Parse(fila.Cells["Total"].Value.ToString(), System.Globalization.NumberStyles.Currency); // Calculado en el procedimiento
                            string observaciones = fila.Cells["Observaciones"].Value.ToString();

                            int idProducto = ObtenerIDProducto(productoNombre);
                            int idProveedor = ObtenerIDProveedor(proveedorNombre);

                            if (idProducto == 0)
                            {
                                MessageBox.Show($"Producto '{productoNombre}' no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                throw new Exception($"Producto '{productoNombre}' no existe.");
                            }

                            if (idProveedor == 0)
                            {
                                MessageBox.Show($"Proveedor '{proveedorNombre}' no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                throw new Exception($"Proveedor '{proveedorNombre}' no existe.");
                            }

                            // Llamar al procedimiento almacenado sp_AltaInventario
                            using (SqlCommand cmd = new SqlCommand("sp_AltaInventario", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@ID_Producto", SqlDbType.Int).Value = idProducto;
                                cmd.Parameters.Add("@CantidadEntrada", SqlDbType.Int).Value = cantidadEntrada;
                                cmd.Parameters.Add("@PrecioPorUnidad", SqlDbType.Decimal).Value = precioPorUnidad;
                                cmd.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = idProveedor;
                                cmd.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 255).Value = observaciones;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        conn.Close();
                    }

                    scope.Complete(); // Confirmar la transacción
                }

                MessageBox.Show("Entradas al inventario añadidas exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                diseñoInventario.DgvInventario.Rows.Clear();
                ActualizarTotales();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al añadir entradas al inventario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductos()
        {
            try
            {
                var productos = db.Producto.Select(p => p.Nombre).ToList();
                diseñoInventario.CmbProducto.Items.Clear();
                diseñoInventario.CmbProducto.Items.AddRange(productos.ToArray());
                diseñoInventario.CmbProducto.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProveedores()
        {
            try
            {
                var proveedores = db.Proveedor.Select(p => p.Nombre).ToList();
                diseñoInventario.CmbProveedor.Items.Clear();
                diseñoInventario.CmbProveedor.Items.AddRange(proveedores.ToArray());
                diseñoInventario.CmbProveedor.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            diseñoInventario.DgvInventario.Columns.Clear();
            diseñoInventario.DgvInventario.Columns.Add("Producto", "Producto");
            diseñoInventario.DgvInventario.Columns.Add("Proveedor", "Proveedor");
            diseñoInventario.DgvInventario.Columns.Add("Fecha", "Fecha");
            diseñoInventario.DgvInventario.Columns.Add("Cantidad Entrada", "Cantidad Entrada");
            diseñoInventario.DgvInventario.Columns.Add("Precio por Unidad", "Precio por Unidad");
            diseñoInventario.DgvInventario.Columns.Add("Total", "Total");
            diseñoInventario.DgvInventario.Columns.Add("Observaciones", "Observaciones");
            diseñoInventario.DgvInventario.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            diseñoInventario.DgvInventario.Columns["Precio por Unidad"].DefaultCellStyle.Format = "C2";
            diseñoInventario.DgvInventario.Columns["Total"].DefaultCellStyle.Format = "C2";
            diseñoInventario.DgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            diseñoInventario.DgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            diseñoInventario.DgvInventario.ReadOnly = true;
            diseñoInventario.DgvInventario.AllowUserToAddRows = false;
        }

        private decimal ObtenerCostoPorProducto(string productoNombre)
        {
            try
            {
                var producto = db.Producto.SingleOrDefault(p => p.Nombre == productoNombre);
                return producto?.Precio ?? 0.00m;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el costo del producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0.00m;
            }
        }

        private void CmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            string productoSeleccionado = diseñoInventario.CmbProducto.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(productoSeleccionado))
            {
                decimal stockDisponible = ObtenerStockDisponible(productoSeleccionado);
                diseñoInventario.LblStockDisponible.Text = $"Stock Disponible: {stockDisponible}";
            }
            else
            {
                diseñoInventario.LblStockDisponible.Text = "Stock Disponible: N/A";
            }
        }

        private decimal ObtenerStockDisponible(string productoNombre)
        {
            try
            {
                var producto = db.Producto.SingleOrDefault(p => p.Nombre == productoNombre);
                return producto?.Stock ?? 0.00m;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al obtener el stock del producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0.00m;
            }
        }

        private int ObtenerIDProducto(string productoNombre)
        {
            var producto = db.Producto.SingleOrDefault(p => p.Nombre == productoNombre);
            return producto?.ID_Producto ?? 0;
        }

        private int ObtenerIDProveedor(string proveedorNombre)
        {
            var proveedor = db.Proveedor.SingleOrDefault(p => p.Nombre == proveedorNombre);
            return proveedor?.ID_Proveedor ?? 0;
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

        private void LimpiarCampos()
        {
            diseñoInventario.CmbProducto.SelectedIndex = -1;
            diseñoInventario.CmbProveedor.SelectedIndex = -1;
            diseñoInventario.DtpFecha.Value = DateTime.Now;
            diseñoInventario.TxtObservaciones.Clear();
            diseñoInventario.TxtCantidadEntrada.Clear();
            diseñoInventario.LblStockDisponible.Text = "Stock Disponible: N/A";
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            diseñoInventario.DgvInventario.Rows.Clear();
            ActualizarTotales();
        }

        private void DgvInventario_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal subtotal = CalcularSubtotal();
            decimal iva = CalcularIVA();
            decimal total = CalcularTotal();

            diseñoInventario.LblSubtotal.Text = $"Subtotal: {subtotal:C2}";
            diseñoInventario.LblIVA.Text = $"IVA (16%): {iva:C2}";
            diseñoInventario.LblTotal.Text = $"Total: {total:C2}";
        }

        private decimal CalcularSubtotal()
        {
            decimal subtotal = 0m;
            foreach (DataGridViewRow row in diseñoInventario.DgvInventario.Rows)
            {
                if (row.IsNewRow) continue;
                decimal totalProducto = decimal.Parse(row.Cells["Total"].Value.ToString(), System.Globalization.NumberStyles.Currency);
                subtotal += totalProducto;
            }
            return subtotal;
        }

        private decimal CalcularIVA()
        {
            decimal subtotal = CalcularSubtotal();
            decimal iva = subtotal * 0.16m; // IVA del 16%
            return iva;
        }

        private decimal CalcularTotal()
        {
            decimal subtotal = CalcularSubtotal();
            decimal iva = CalcularIVA();
            return subtotal + iva;
        }

        private void BtnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                var inventarios = from inv in db.Inventarios
                                  join prod in db.Producto on inv.ID_Producto equals prod.ID_Producto
                                  join prov in db.Proveedor on inv.ID_Proveedor equals prov.ID_Proveedor
                                  select new
                                  {
                                      Producto = prod.Nombre,
                                      Proveedor = prov.Nombre,
                                      FechaEntrada = inv.FechaEntrada,
                                      CantidadEntrada = inv.CantidadEntrada,
                                      PrecioPorUnidad = inv.PrecioPorUnidad,
                                      Total = inv.Total,
                                      Observaciones = inv.Observaciones
                                  };

                var listaInventarios = inventarios.ToList();

                if (listaInventarios.Any())
                {
                    string mensaje = "Entradas de Inventario encontradas:\n";
                    foreach (var item in listaInventarios)
                    {
                        mensaje += $"Producto: {item.Producto}, Proveedor: {item.Proveedor}, Fecha de Entrada: {item.FechaEntrada?.ToShortDateString() ?? "N/A"}, " +
                                   $"Cantidad Entrada: {item.CantidadEntrada}, Precio por Unidad: {item.PrecioPorUnidad:C2}, " +
                                   $"Total: {item.Total:C2}, Observaciones: {item.Observaciones}\n";
                    }
                    MessageBox.Show(mensaje, "Entradas de Inventario Encontradas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontraron entradas de inventario.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                MessageBox.Show("No tienes permisos para realizar consultas de inventario.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la consulta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (diseñoInventario.DgvInventario.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = diseñoInventario.DgvInventario.SelectedRows[0];

                DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este producto?",
                                                            "Confirmar Eliminación",
                                                            MessageBoxButtons.YesNo,
                                                            MessageBoxIcon.Question);
                if (confirmacion == DialogResult.Yes)
                {
                    diseñoInventario.DgvInventario.Rows.Remove(filaSeleccionada);
                    MessageBox.Show("Producto eliminado del inventario.", "Producto Eliminado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ActualizarTotales(); // Actualizar totales después de eliminar
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.", "Seleccione un Producto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
