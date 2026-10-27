using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    //Creacion de clase de apoyo para convertir las listas y asi evitar poner en pacintes y medicos 
    public static class TextoLista
    {
        private static IEnumerable<string> Separar(string texto)
        {
            return (texto ?? "")
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t != "");
        }

        public static List<string> Telefonos(string texto)
        {
            return Separar(texto)
                .Select(t => t.Replace("-", "").Replace(" ", ""))   // acepta 7712-3456
                .Distinct().ToList();
        }

        public static List<string> Correos(string texto)
        {
            return Separar(texto).Distinct().ToList();
        }
    }
}
