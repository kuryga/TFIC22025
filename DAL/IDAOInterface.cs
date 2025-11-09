using System.Collections.Generic;

namespace DAL.Seguridad.DV
{
    public interface IDAOInterface<T>
    {
        List<T> GetAll();

        void Create(T obj);

        void Update(T obj);
    }
}
