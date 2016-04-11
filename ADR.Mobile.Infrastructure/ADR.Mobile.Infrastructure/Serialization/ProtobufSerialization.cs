using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ADR.Mobile.Infrastructure.Serialization
{
    public class ProtobufSerialization<T> : IObjectSerialization<T> where T : class
    {
        public static object Log1 = new object();
        public static object Log2 = new object();
        public static object Log3 = new object();
        public static object LogSetConnection = new object();

        public byte[] Serialize(T objectGraph)
        {
            lock (Log1)
            {
                if (objectGraph != null)
                {
                    try
                    {
                        var ms = new MemoryStream();
                        Serializer.Serialize(ms, objectGraph);
                        var rt = ms.ToArray();
                        return rt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to serialize object", ex);
                    }
                }
                else
                {
                    throw new Exception("Object serialize is null");
                }
            }
        }

        public T DeSerialize(byte[] data)
        {
            lock (Log2)
            {
                if (data != null && data.Length > 0)
                {
                    try
                    {
                        var ms = new MemoryStream(data);
                        return Serializer.Deserialize<T>(ms);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception("Unable to deserialize object", ex);
                        return default(T);
                    }
                }
                else
                {
                    //throw new Exception("Object Deserialize is null or empty");
                    return default(T);
                }
            }
        }
    }
}
