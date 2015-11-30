using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shard.HashingFunction
{
    public interface IHashing
    {
        UInt32 Hash(Byte[] data);
    }
}
