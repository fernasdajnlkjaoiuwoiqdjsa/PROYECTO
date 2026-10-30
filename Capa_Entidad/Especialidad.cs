using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capoa_Entidad
{
    public class Especialidad
    {
        public int id_especialidad {  get; set; }
        public string NombreEspecialidad {  get; set; }

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
