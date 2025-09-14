using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE {
    public interface ICrud<T> {
        bool Create(T objAdd);
        List<T> GetAll();
        T GetByUsername(String Username);
        bool Update(T objUpd);
        bool Delete(T objUdp);
    }
}
