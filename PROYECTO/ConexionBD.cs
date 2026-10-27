using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public class ConexionBD
    {
        private string cadenaConexion = @"server = localhost; integrated security = true; database=POOCATEDRA;";
        private SqlConnection conexion;

        public SqlConnection AbrirConexion()
        {
            conexion = new SqlConnection(cadenaConexion);
            conexion.Open();
            return conexion;
        }

        public void CerrarConxion()
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
                conexion.Close();
        }

    }
}
