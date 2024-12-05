using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Transactions; // Importar el espacio de nombres para TransactionScope

namespace ProgAbarrotesDB
{
    public partial class FormClientes : Form
    {
        private DiseñoClientes diseñoClientes;
        private ClaseConexion conexion;

        public FormClientes(ClaseConexion conexionDb)
        {
            InitializeComponent();
            InitializeForm();
            conexion = conexionDb;
            CargarDatos();
        }

        private void InitializeForm()
        {
            // Inicializar y agregar el diseño de clientes
            diseñoClientes = new DiseñoClientes(this);

            // Asignar eventos a los botones
            AsignarEventos();
        }

        private void AsignarEventos()
        {
            diseñoClientes.BtnAlta.Click += BtnAlta_Click;
            diseñoClientes.BtnConsulta.Click += BtnConsulta_Click;
            diseñoClientes.BtnModificar.Click += BtnModificar_Click;
            diseñoClientes.BtnEliminar.Click += BtnEliminar_Click;
        }

        // Evento para el botón Alta
        private void BtnAlta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoClientes.TxtNombre.Text.Trim();
            string telefono = diseñoClientes.TxtTelefono.Text.Trim();
            string direccion = diseñoClientes.TxtDireccion.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(direccion))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DataClasses1DataContext db = conexion.GetDataContext();

                    Cliente nuevoCliente = new Cliente
                    {
                        Nombre = nombre,
                        Telefono = telefono,
                        Direccion = direccion
                    };

                    db.Cliente.InsertOnSubmit(nuevoCliente);
                    db.SubmitChanges();

                    // Completa la transacción
                    scope.Complete();
                }

                MessageBox.Show("Cliente agregado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar campos y recargar datos
                diseñoClientes.TxtNombre.Clear();
                diseñoClientes.TxtTelefono.Clear();
                diseñoClientes.TxtDireccion.Clear();
                CargarDatos();
            }
            catch (SqlException ex)
            {
                HandleSqlException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para manejar excepciones SQL específicas
        private void HandleSqlException(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627: // Violación de clave primaria
                    MessageBox.Show("Ya existe un cliente con los mismos datos únicos (ID o teléfono).", "Error de Clave Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 547: // Violación de clave foránea o de restricción
                    MessageBox.Show("No se puede realizar la acción debido a restricciones de integridad referencial.", "Restricción de Integridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 2601: // Violación de índice único
                    MessageBox.Show("Un valor único ya existe y está duplicado en el sistema.", "Duplicación de Índice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 229: // Permisos insuficientes
                    MessageBox.Show("No tienes permisos para realizar esta operación.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Ocurrió un error de base de datos: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // Evento para el botón Consulta
        private void BtnConsulta_Click(object sender, EventArgs e)
        {
            string nombre = diseñoClientes.TxtNombre.Text.Trim();
            string telefono = diseñoClientes.TxtTelefono.Text.Trim();

            if (string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(telefono))
            {
                MessageBox.Show("Por favor, ingrese al menos un criterio de búsqueda (Nombre o Teléfono).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DataClasses1DataContext db = conexion.GetDataContext();

                // Filtrar según los criterios ingresados
                var clientes = from c in db.Cliente
                               where (string.IsNullOrEmpty(nombre) || c.Nombre.Contains(nombre)) &&
                                     (string.IsNullOrEmpty(telefono) || c.Telefono.Contains(telefono))
                               select new
                               {
                                   c.ID_Cliente,
                                   c.Nombre,
                                   c.Telefono,
                                   c.Direccion
                               };

                // Asignar los datos al DataGridView
                diseñoClientes.DgvClientes.DataSource = clientes.ToList();

                // Verificar si se encontraron resultados
                if (clientes.Any())
                {
                    MessageBox.Show("Consulta realizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontraron clientes que coincidan con los criterios ingresados.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Evento para el botón Modificar
        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (diseñoClientes.DgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un cliente para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoClientes.DgvClientes.SelectedRows[0];
            int idCliente = Convert.ToInt32(filaSeleccionada.Cells["ID_Cliente"].Value);

            string nombreActual = filaSeleccionada.Cells["Nombsre"].Value.ToString();
            string telefonoActual = filaSeleccionada.Cells["Telefono"].Value.ToString();
            string direccionActual = filaSeleccionada.Cells["Direccion"].Value.ToString();

            // Abrir el formulario de edición
            FormEditarCliente formEditar = new FormEditarCliente(idCliente, nombreActual, telefonoActual, direccionActual, conexion);
            var resultado = formEditar.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                try
                {
                    CargarDatos();
                    MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Si el resultado es Cancel, no hacer nada
        }

        // Evento para el botón Eliminar
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (diseñoClientes.DgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un cliente para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = diseñoClientes.DgvClientes.SelectedRows[0];
            int idCliente = Convert.ToInt32(filaSeleccionada.Cells["ID_Cliente"].Value);

            DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este cliente?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        DataClasses1DataContext db = conexion.GetDataContext();
                        Cliente cliente = db.Cliente.SingleOrDefault(c => c.ID_Cliente == idCliente);

                        if (cliente != null)
                        {
                            db.Cliente.DeleteOnSubmit(cliente);
                            db.SubmitChanges();

                            // Completa la transacción
                            scope.Complete();

                            MessageBox.Show("Cliente eliminado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    HandleSqlException(ex);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Método para cargar datos en el DataGridView
        private void CargarDatos()
        {
            try
            {
                DataClasses1DataContext db = conexion.GetDataContext();

                var clientes = from c in db.Cliente
                               select new
                               {
                                   c.ID_Cliente,
                                   c.Nombre,
                                   c.Telefono,
                                   c.Direccion
                               };

                diseñoClientes.DgvClientes.DataSource = clientes.ToList();
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
    }
}
