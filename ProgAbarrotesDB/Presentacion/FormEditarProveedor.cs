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
    public partial class FormEditarProveedor : Form
    {
        private int idProveedor;
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

        public FormEditarProveedor(int id, string nombre, string telefono, string direccion, ClaseConexion conexionDb)
        {
            InitializeComponent();
            InitializeComp();
            idProveedor = id;
            conexion = conexionDb;

            // Asignar los valores actuales a los TextBoxes
            txtNombre.Text = nombre;
            txtTelefono.Text = telefono;
            txtDireccion.Text = direccion;
        }

        private void InitializeComp()
        {
            // Configuración general del formulario
            this.Text = "Editar Proveedor";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Inicializar el TableLayoutPanel
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.RowCount = 5;
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Padding = new Padding(20);
            tableLayoutPanel.AutoSize = true;
            tableLayoutPanel.AutoScroll = true;

            // Configurar filas y columnas
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Labels
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // TextBoxes

            for (int i = 0; i < 5; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }

            // Título
            lblTitulo = new Label();
            lblTitulo.Text = "Editar Información del Proveedor";
            lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(0, 122, 204); // Azul
            lblTitulo.AutoSize = true;
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            tableLayoutPanel.SetColumnSpan(lblTitulo, 2);
            tableLayoutPanel.Controls.Add(lblTitulo, 0, 0);

            // Label y TextBox para Nombre
            lblNombre = new Label();
            lblNombre.Text = "Nombre:";
            lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblNombre.ForeColor = Color.Black;
            lblNombre.TextAlign = ContentAlignment.MiddleRight;
            lblNombre.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(lblNombre, 0, 1);

            txtNombre = new TextBox();
            txtNombre.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txtNombre.Dock = DockStyle.Fill;
            txtNombre.BackColor = Color.White;
            tableLayoutPanel.Controls.Add(txtNombre, 1, 1);

            // Label y TextBox para Teléfono
            lblTelefono = new Label();
            lblTelefono.Text = "Teléfono:";
            lblTelefono.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblTelefono.ForeColor = Color.Black;
            lblTelefono.TextAlign = ContentAlignment.MiddleRight;
            lblTelefono.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(lblTelefono, 0, 2);

            txtTelefono = new TextBox();
            txtTelefono.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txtTelefono.Dock = DockStyle.Fill;
            txtTelefono.BackColor = Color.White;
            tableLayoutPanel.Controls.Add(txtTelefono, 1, 2);

            // Label y TextBox para Dirección
            lblDireccion = new Label();
            lblDireccion.Text = "Dirección:";
            lblDireccion.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblDireccion.ForeColor = Color.Black;
            lblDireccion.TextAlign = ContentAlignment.MiddleRight;
            lblDireccion.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(lblDireccion, 0, 3);

            txtDireccion = new TextBox();
            txtDireccion.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txtDireccion.Dock = DockStyle.Fill;
            txtDireccion.BackColor = Color.White;
            tableLayoutPanel.Controls.Add(txtDireccion, 1, 3);

            // Botón Guardar
            btnGuardar = new Button();
            btnGuardar.Text = "Guardar";
            btnGuardar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnGuardar.BackColor = Color.FromArgb(0, 122, 204); // Azul
            btnGuardar.ForeColor = Color.White;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Height = 35;
            btnGuardar.Cursor = Cursors.Hand;
            btnGuardar.Dock = DockStyle.Fill;
            btnGuardar.Click += btnGuardar_Click;
            tableLayoutPanel.Controls.Add(btnGuardar, 0, 4);

            // Botón Cancelar
            btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancelar.BackColor = Color.Gray;
            btnCancelar.ForeColor = Color.White;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Height = 35;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.Dock = DockStyle.Fill;
            btnCancelar.Click += btnCancelar_Click;
            tableLayoutPanel.Controls.Add(btnCancelar, 1, 4);

            // Agregar el TableLayoutPanel al formulario
            this.Controls.Add(tableLayoutPanel);
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
                    string connectionString = conexion.GetDataContext().Connection.ConnectionString;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand("sp_UpdateProveedor", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Agregar parámetros con tipos específicos
                            cmd.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = idProveedor;
                            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nuevoNombre;
                            cmd.Parameters.Add("@Telefono", SqlDbType.NVarChar, 15).Value = nuevoTelefono;
                            cmd.Parameters.Add("@Direccion", SqlDbType.NVarChar, 255).Value = nuevaDireccion;

                            int filasAfectadas = cmd.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                scope.Complete(); // Confirmar la transacción

                                MessageBox.Show("Proveedor modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("No se encontró el proveedor para actualizar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 229)
            {
                MessageBox.Show("No tienes permisos para modificar proveedores.", "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                // Manejo de errores específicos de SQL Server
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
                    default:
                        MessageBox.Show($"Ocurrió un error de base de datos: {ex.Message}", "Error en la Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al modificar el proveedor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento para el botón Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
