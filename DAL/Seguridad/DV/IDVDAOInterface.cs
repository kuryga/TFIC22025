using DigitosVerificadoresLib.services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitosVerificadoresLib
{
    public interface IDVDAOInterface<T>
    {
        List<T> GetAll();

        String CalculateHorizontal(T obj);

        String CalculateVertical(List<T> list);

        void Save(T obj);

        void Update(T obj);
        void UpdateVertical();

        void UpdateAllDV();

        string GetVertical();
    }
}
