using System.Collections.Generic;

namespace BE
{
    public interface ICrud<T>
    {
        bool Create(T objAdd);
        List<T> GetAll();
        bool Update(T objUpd);
        bool Deshabilitar(int idObj, bool deshabilitar);
    }
}