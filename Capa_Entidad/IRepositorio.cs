using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    //Interfaz que define los métodos que deben implementar las clases que la implementen
    public interface IRepositorio<T>
    {
        //Método para agregar un objeto de tipo T al repositorio
        void Guardar(T entidad);
        //  //Método para Buscar Filtar un objeto de tipo T del repositorio
        T Buscarr(int id);
        //Método para Actualizar un objeto de tipo T del repositorio
        void Actualizar(T entidad);
        //Método para eliminar un objeto de tipo T del repositorio
        void Eliminar(int id);
        //Método para listar todos los objetos de tipo T del repositorio
        List<T> ListarTodos();

      

    }
}
