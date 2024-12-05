using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgAbarrotesDB
{
    public class ClaseConexion
    {
        private DataClasses1DataContext db;

        /// <summary>
        /// Constructor que acepta una cadena de conexión.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a la base de datos.</param>
        public ClaseConexion(string connectionString)
        {
            db = new DataClasses1DataContext(connectionString);
        }

        /// <summary>
        /// Obtiene el DataContext para realizar operaciones de base de datos.
        /// </summary>
        /// <returns>Instancia de DataClasses1DataContext.</returns>
        public DataClasses1DataContext GetDataContext()
        {
            return db;

        }


    }

}
