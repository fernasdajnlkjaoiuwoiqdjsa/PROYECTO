using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capoa_Entidad
{
    public class Especialidad
    {
        //Propiedades de la clase Especialidad referenciando  a la base de datos
        public int id_especialidad {  get; set; }
        public string NombreEspecialidad {  get; set; }
        //Constructor de la clase Especialidad con parámetros para inicializar las propiedades
        public Especialidad(int idEspecialidad, string nombreespe)
        {
            id_especialidad = idEspecialidad;
            NombreEspecialidad = nombreespe;
        }
        public Especialidad()
        {

        }
    }
}
