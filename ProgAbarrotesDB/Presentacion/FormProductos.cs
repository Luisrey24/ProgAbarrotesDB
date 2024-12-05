using ProgAbarrotesDB.Clases;
using ProgAbarrotesDB.Components;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace ProgAbarrotesDB.Forms
{
    public partial class FormProductos : Form
    {
        private DiseñoProductos diseñoProductos;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormProductos(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext(); // Crear una instancia única de DataContext
            InitializeForm();
            CargarDatos();

            this.FormClosed += FormProductos_FormClosed;
        }

        private void InitializeForm()
        {
            diseñoProductos = new DiseñoProductos(this);
            AsignarEventos();
        }

        private void AsignarEventos()
        {
            // Asignar los eventos a los botones del diseño
            diseñoProductos.BtnAlta.Click += BtnAlta_Click;
            diseñoProductos.BtnConsulta.Click += BtnConsulta_Click;
            diseñoProductos.BtnModificar.Click += BtnModificar_Click;
            diseñoProductos.BtnEliminar.Click += BtnEliminar_Click;
        }

        private void BtnAlta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoProductos.TxtNombre.Text.Trim();
            string precioText = diseñoProductos.TxtPrecio.Text.Trim();
            string descripcion = diseñoProductos.TxtDescripcion.Text.Trim();
            string stockText = diseñoProductos.TxtStock.Text.Trim(); // Nuevo campo

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(precioText) || string.IsNullOrEmpty(descripcion) || string.IsNullOrEmpty(stockText))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(precioText, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Por favor, ingrese un precio válido y mayor a 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(stockText, out int stock) || stock < 0)
            {
                MessageBox.Show("Por favor, ingrese un stock válido (entero y no negativo).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string connectionString = db.Connection.ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_AltaProducto", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Stock", stock);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Producto agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LimpiarCampos();
                CargarDatos();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            diseñoProductos.TxtNombre.Clear();
            diseñoProductos.TxtPrecio.Clear();
            diseñoProductos.TxtDescripcion.Clear();
            diseñoProductos.TxtStock.Clear(); // Limpiar el campo Stock
        }

        private void BtnConsulta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoProductos.TxtNombre.Text.Trim();
            string precioText = diseñoProductos.TxtPrecio.Text.Trim();
            string stockText = diseñoProductos.TxtStock.Text.Trim(); // Opcional para filtrar por stock

            decimal precio = 0;
            bool filtrarPrecio = !string.IsNullOrEmpty(precioText) && decimal.TryParse(precioText, out precio);

            int stock = 0;
            bool filtrarStock = !string.IsNullOrEmpty(stockText) && int.TryParse(stockText, out stock);

            if (string.IsNullOrEmpty(nombre) && !filtrarPrecio && !filtrarStock)
            {
                MessageBox.Show("Por favor, ingrese al menos un criterio de búsqueda (Nombre, Precio o Stock).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var productos = from p in db.Producto
                                where (string.IsNullOrEmpty(nombre) || p.Nombre.Contains(nombre)) &&
                                      (!filtrarPrecio || p.Precio == precio) &&
                                      (!filtrarStock || p.Stock == stock)
                                select new
                                {
                                    p.ID_Producto,
                                    p.Nombre,
                                    p.Precio,
                                    p.Stock, // Incluir Stock en la consulta
                                    p.Descripcion
                                };

                // Asignar el resultado al DataSource sin configurar columnas nuevamente
                diseñoProductos.DgvProductos.DataSource = productos.ToList();

                MessageBox.Show(productos.Any() ? "Consulta realizada exitosamente." : "No se encontraron productos que coincidan con los criterios ingresados.",
                                "Resultado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la consulta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (diseñoProductos.DgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoProductos.DgvProductos.SelectedRows[0];
            int idProducto = Convert.ToInt32(filaSeleccionada.Cells["ID_Producto"].Value);

            string nombreActual = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string precioActual = filaSeleccionada.Cells["Precio"].Value.ToString();
            string descripcionActual = filaSeleccionada.Cells["Descripcion"].Value.ToString();
            string stockActual = filaSeleccionada.Cells["Stock"].Value.ToString(); // Obtener Stock actual

            FormEditarProducto formEditar = new FormEditarProducto(idProducto, nombreActual, precioActual, descripcionActual, stockActual, conexion);
            formEditar.ShowDialog();

            try
            {
                CargarDatos();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (diseñoProductos.DgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoProductos.DgvProductos.SelectedRows[0];
            int idProducto = Convert.ToInt32(filaSeleccionada.Cells["ID_Producto"].Value);

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    string connectionString = db.Connection.ConnectionString;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand("sp_BajaProducto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID_Producto", idProducto);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarDatos();
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarDatos()
        {
            try
            {
                var productos = from p in db.Producto
                                select new
                                {
                                    p.ID_Producto,
                                    p.Nombre,
                                    p.Precio,
                                    p.Stock, // Incluir Stock en la consulta
                                    p.Descripcion
                                };

                // Asignar el resultado al DataSource sin configurar columnas nuevamente
                diseñoProductos.DgvProductos.DataSource = productos.ToList();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627: // Violación de clave primaria o índice único
                    MessageBox.Show("Error: Registro duplicado detectado.", "Error de Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 547: // Violación de integridad referencial
                    MessageBox.Show("Error: Violación de integridad referencial.", "Restricción de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 2601: // Duplicación de índice único
                    MessageBox.Show("Error: Índice único duplicado.", "Error de Índice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 229: // Permisos insuficientes
                    MessageBox.Show("No tienes permisos para realizar esta operación.", "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Error de base de datos: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // Evento para liberar el DataContext cuando se cierre el formulario
        private void FormProductos_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose(); // Liberar el DataContext si está en uso
        }
    }
}
