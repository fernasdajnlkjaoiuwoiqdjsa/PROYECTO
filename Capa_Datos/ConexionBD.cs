using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;



namespace Capa_Datos
{
    // Clase que se encarga solo de abrir y cerrar la conexión a SQL Server. La hice para tener la
    // cadena de conexión en un único lugar: todos los repositorios (Cita, Paciente, Medico,
    // Receta, Usuario, Especialidad) la usan con AbrirConexion() y CerrarConxion(), y si cambio
    // de servidor o de base de datos solo modifico esta clase. No ejecuta consultas, solo
    // entrega la conexión.
    public class ConexionBD
    {
        // Cadena de conexión: servidor local, autenticación de Windows (integrated security, así
        // no guardo usuario ni contraseña de SQL en el código) y la base de datos POOCATEDRA.
        private string cadenaConexion = @"server = localhost; integrated security = true; database=POOCATEDRA;";
        // Guarda la conexión que se abrió, para poder cerrarla después desde CerrarConxion().
        private SqlConnection conexion;

        // Crea una conexión nueva con la cadena de arriba, la abre y la devuelve para que el
        // repositorio la use en su SqlCommand. Si el servidor no está disponible o la base no
        // existe, Open() lanza una excepción aquí mismo.
        public SqlConnection AbrirConexion()
        {
            conexion = new SqlConnection(cadenaConexion);
            conexion.Open();
            return conexion;
        }

        // Cierra la conexión si está abierta. Los repositorios la llaman en el finally para que
        // quede cerrada aunque haya fallado la consulta. Reviso que no sea null y que el estado
        // sea Open para no cerrar una conexión que no existe o que ya estaba cerrada.
        // (El nombre lleva el typo "Conxion" y lo dejé igual porque los repositorios lo llaman.)
        public void CerrarConxion()
        {
            if (conexion != null && conexion.State == ConnectionState.Open)
                conexion.Close();
        }

    }
}