using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public interface IRepository<T>
    {
        void Save(T aggregate);
        T Find(int id);
        int SaveChanges();
    }
}
