using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Transactions; // Importar para TransactionScope
using System.Data.SqlClient; // Importar para SqlException

namespace ProgAbarrotesDB
{
    public partial class FormEditarCliente : Form
    {
        private int idCliente;
        private ClaseConexion conexion;

        // Controles del formulario
        private Label lblTitulo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Button btnGuardar;
        private Button btnCancelar;
        private TableLayoutPanel tableLayoutPanel;

        public FormEditarCliente(int id, string nombre, string telefono, string direccion, ClaseConexion conexionDb)
        {
            InitializeComponent();
            Component();
            idCliente = id;
            conexion = conexionDb;

            // Inicializa los TextBoxes después de que han sido creados en InitializeComponent
            txtNombre.Text = nombre;
            txtTelefono.Text = telefono;
            txtDireccion.Text = direccion;
        }

        private void Component()
        {
            // Configuración general del formulario
            this.Text = "Editar Cliente";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Inicializar el TableLayoutPanel
            tableLayoutPanel = new TableLayoutPanel
            {
                RowCount = 5,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoSize = true,
                AutoScroll = true
            };

            // Configurar filas y columnas
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            for (int i = 0; i < 5; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }

            // Título
            lblTitulo = new Label
            {
                Text = "Editar Información del Cliente",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 204),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            tableLayoutPanel.SetColumnSpan(lblTitulo, 2);
            tableLayoutPanel.Controls.Add(lblTitulo, 0, 0);

            // Label y TextBox para Nombre
            lblNombre = CrearLabel("Nombre:");
            txtNombre = CrearTextBox();
            tableLayoutPanel.Controls.Add(lblNombre, 0, 1);
            tableLayoutPanel.Controls.Add(txtNombre, 1, 1);

            // Label y TextBox para Teléfono
            lblTelefono = CrearLabel("Teléfono:");
            txtTelefono = CrearTextBox();
            tableLayoutPanel.Controls.Add(lblTelefono, 0, 2);
            tableLayoutPanel.Controls.Add(txtTelefono, 1, 2);

            // Label y TextBox para Dirección
            lblDireccion = CrearLabel("Dirección:");
            txtDireccion = CrearTextBox();
            tableLayoutPanel.Controls.Add(lblDireccion, 0, 3);
            tableLayoutPanel.Controls.Add(txtDireccion, 1, 3);

            // Botón Guardar
            btnGuardar = CrearBoton("Guardar", Color.FromArgb(0, 122, 204), btnGuardar_Click);
            tableLayoutPanel.Controls.Add(btnGuardar, 0, 4);

            // Botón Cancelar
            btnCancelar = CrearBoton("Cancelar", Color.Gray, btnCancelar_Click);
            tableLayoutPanel.Controls.Add(btnCancelar, 1, 4);

            // Agregar el TableLayoutPanel al formulario
            this.Controls.Add(tableLayoutPanel);
        }

        // Método para crear etiquetas
        private Label CrearLabel(string texto)
        {
            return new Label
            {
                Text = texto,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };
        }

        // Método para crear cuadros de texto
        private TextBox CrearTextBox()
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
        }

        // Método para crear botones
        private Button CrearBoton(string texto, Color color, EventHandler eventoClick)
        {
            var boton = new Button
            {
                Text = texto,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 35,
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill
            };
            boton.FlatAppearance.BorderSize = 0;

            // Asignación del evento Click con +=
            boton.Click += eventoClick;

            return boton;
        }

        // Evento para el botón Guardar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nuevoNombre = txtNombre.Text.Trim();
            string nuevoTelefono = txtTelefono.Text.Trim();
            string nuevaDireccion = txtDireccion.Text.Trim();

            // Validaciones
            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevoTelefono) || string.IsNullOrEmpty(nuevaDireccion))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validación del formato del teléfono (solo números y longitud entre 7 y 15)
            if (!System.Text.RegularExpressions.Regex.IsMatch(nuevoTelefono, @"^\d{7,15}$"))
            {
                MessageBox.Show("Por favor, ingrese un número de teléfono válido (solo números, entre 7 y 15 dígitos).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DataClasses1DataContext db = conexion.GetDataContext();

                    // Buscar el cliente por ID
                    var cliente = db.Cliente.SingleOrDefault(c => c.ID_Cliente == idCliente);
                    if (cliente != null)
                    {
                        cliente.Nombre = nuevoNombre;
                        cliente.Telefono = nuevoTelefono;
                        cliente.Direccion = nuevaDireccion;

                        db.SubmitChanges(); // Guardar los cambios en la base de datos

                        // Completa la transacción
                        scope.Complete();

                        // Indicar éxito y cerrar el formulario con DialogResult.OK
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cliente no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // No completar la transacción, lo que causará un ROLLBACK
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
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
                    MessageBox.Show("No tienes permisos para realizar esta operación.", "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                default:
                    MessageBox.Show($"Ocurrió un error de base de datos: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // Evento para el botón Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
