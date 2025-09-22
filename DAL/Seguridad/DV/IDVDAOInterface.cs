using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Seguridad.DV
{
    public interface IDAOInterface<T>
    {
        List<T> GetAll();

        void Create(T obj);

        void Update(T obj);
    }
}
