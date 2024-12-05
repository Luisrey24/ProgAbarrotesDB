using System;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Windows.Forms;
using ProgAbarrotesDB.Components;

namespace ProgAbarrotesDB
{
    public partial class FormProveedor : Form
    {
        private DiseñoProveedor diseñoProveedor;
        private ClaseConexion conexion;
        private DataClasses1DataContext db;

        public FormProveedor(ClaseConexion conexionDb)
        {
            InitializeComponent();
            conexion = conexionDb;
            db = conexion.GetDataContext(); // Inicializar DataContext
            InitializeForm();
            CargarDatos();

            // Liberar DataContext al cerrar el formulario
            this.FormClosed += FormProveedor_FormClosed;
        }

        private void FormProveedor_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose(); // Liberar DataContext
        }

        private void InitializeForm()
        {
            // Inicializar y agregar el diseño de proveedores
            diseñoProveedor = new DiseñoProveedor(this);
            AsignarEventos();
        }

        private void AsignarEventos()
        {
            diseñoProveedor.BtnAlta.Click += BtnAlta_Click;
            diseñoProveedor.BtnConsulta.Click += BtnConsulta_Click;
            diseñoProveedor.BtnModificar.Click += BtnModificar_Click;
            diseñoProveedor.BtnEliminar.Click += BtnEliminar_Click;
        }

        private void CargarDatos()
        {
            try
            {
                var proveedores = from p in db.Proveedor
                                  select new
                                  {
                                      p.ID_Proveedor,
                                      p.Nombre,
                                      p.Telefono,
                                      p.Direccion
                                  };

                diseñoProveedor.DgvProveedores.DataSource = proveedores.ToList();
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

        private void BtnAlta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoProveedor.TxtNombre.Text.Trim();
            string telefono = diseñoProveedor.TxtTelefono.Text.Trim();
            string direccion = diseñoProveedor.TxtDireccion.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(direccion))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Proveedor nuevoProveedor = new Proveedor
                    {
                        Nombre = nombre,
                        Telefono = telefono,
                        Direccion = direccion
                    };

                    db.Proveedor.InsertOnSubmit(nuevoProveedor);
                    db.SubmitChanges();

                    scope.Complete(); // Confirma la transacción

                    MessageBox.Show("Proveedor agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarDatos();
                }
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnConsulta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoProveedor.TxtNombre.Text.Trim();
            string telefono = diseñoProveedor.TxtTelefono.Text.Trim();

            if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(telefono))
            {
                MessageBox.Show("Por favor, ingrese al menos un criterio de búsqueda (Nombre o Teléfono).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var proveedores = from p in db.Proveedor
                                  where (string.IsNullOrEmpty(nombre) || p.Nombre.Contains(nombre)) &&
                                        (string.IsNullOrEmpty(telefono) || p.Telefono.Contains(telefono))
                                  select new
                                  {
                                      p.ID_Proveedor,
                                      p.Nombre,
                                      p.Telefono,
                                      p.Direccion
                                  };

                diseñoProveedor.DgvProveedores.DataSource = proveedores.ToList();

                if (!proveedores.Any())
                {
                    MessageBox.Show("No se encontraron proveedores que coincidan con los criterios ingresados.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
            if (diseñoProveedor.DgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un proveedor para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoProveedor.DgvProveedores.SelectedRows[0];
            int idProveedor = Convert.ToInt32(filaSeleccionada.Cells["ID_Proveedor"].Value);

            string nombreActual = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string telefonoActual = filaSeleccionada.Cells["Telefono"].Value.ToString();
            string direccionActual = filaSeleccionada.Cells["Direccion"].Value.ToString();

            FormEditarProveedor formEditar = new FormEditarProveedor(idProveedor, nombreActual, telefonoActual, direccionActual, conexion);
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
                MessageBox.Show($"Error al modificar el proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (diseñoProveedor.DgvProveedores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un proveedor para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoProveedor.DgvProveedores.SelectedRows[0];
            int idProveedor = Convert.ToInt32(filaSeleccionada.Cells["ID_Proveedor"].Value);

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este proveedor?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Proveedor proveedor = db.Proveedor.SingleOrDefault(p => p.ID_Proveedor == idProveedor);

                        if (proveedor != null)
                        {
                            db.Proveedor.DeleteOnSubmit(proveedor);
                            db.SubmitChanges();

                            scope.Complete(); // Confirma la transacción

                            MessageBox.Show("Proveedor eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("Proveedor no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627: // Clave duplicada
                    MessageBox.Show("Ya existe un proveedor con los mismos datos únicos.", "Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void LimpiarCampos()
        {
            diseñoProveedor.TxtNombre.Clear();
            diseñoProveedor.TxtTelefono.Clear();
            diseñoProveedor.TxtDireccion.Clear();
        }
    }
}
