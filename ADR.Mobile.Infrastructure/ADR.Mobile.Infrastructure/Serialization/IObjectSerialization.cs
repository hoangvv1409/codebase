using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Serialization
{
    public interface IObjectSerialization<T> where T : class
    {
        byte[] Serialize(T objectGraph);

        T DeSerialize(byte[] data);
    }
}
