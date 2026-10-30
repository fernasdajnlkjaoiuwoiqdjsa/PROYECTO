using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    public interface IRepositorio<T>
    {
        void Guardar(T entidad);
        T Buscarr(int id);
        void Actualizar(T entidad);
        void Eliminar(int id);
        List<T> ListarTodos();

      

    }
}
