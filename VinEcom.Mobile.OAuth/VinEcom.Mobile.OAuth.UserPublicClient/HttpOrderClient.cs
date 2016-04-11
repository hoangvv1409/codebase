using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.OnePointApi.CVLibPublic;
using BusinessObjectPublicUser;

namespace VinEcom.Mobile.OAuth.UserPublicClient
{
    public interface IHttpOrderClient : IBasicClient
    {
        string Saltkey { get; set; }
        bool IsNoneEncrypt { get; set; }
        Task<ResponseObject> PostAsync(string url, Dictionary<string, string> dicparam, object data, string appid, int userid);
        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <author>Hung Tran</author>
        /// <typeparam name="TU">The type of the u.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="url">The URL.</param>
        /// <param name="dicparam">The dicparam.</param>
        /// <param name="appid">The appid.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        ResponseObject Post<TU>(TU request, string url, Dictionary<string, string> dicparam, string appid, int userid);

        ResponseObject MobilePost<TU>(TU request, string url, Dictionary<string, string> dicparam);

        ResponseObject Post(byte[] byteData, string url, Dictionary<string, string> dictionary, string appid, int userid);
        byte[] PostGetByte(byte[] byteData, string url, Dictionary<string, string> dictionary, string appid, int userid);
    }

    public class HttpOrderClient : IHttpOrderClient
    {
        public string Saltkey { get; set; }
        private string Signature { get; set; }
        public bool IsNoneEncrypt { get; set; }

        public string ResponseMessage { get; set; }

        public int Status { get; set; }

        public int Uid { get; set; }

        public HttpOrderClient(string publickey, bool isNoneEncrypt)
        {
            IsNoneEncrypt = isNoneEncrypt;
            if (!IsNoneEncrypt)
            {
                Saltkey = Common.GenerateNonce();
                Signature = Encrypt.EncryptRSA(Saltkey, publickey);
            }
        }

        public async Task<ResponseObject> PostAsync(string url, Dictionary<string, string> dicparam, object data, string appid, int userid)
        {
            if (!dicparam.ContainsKey("userid"))
                dicparam.Add("userid", userid.ToString(CultureInfo.InvariantCulture));
            if (!dicparam.ContainsKey("appid"))
                dicparam.Add("appid", appid);
            if (IsNoneEncrypt)
            {
                if (!dicparam.ContainsKey("noneEncrypt"))
                    dicparam.Add("noneEncrypt", "1");
            }
            else
            {
                if (!dicparam.ContainsKey("signature"))
                    dicparam.Add("signature", Signature);
            }
            if (data == null)
                data = new byte[0];
            var objData = new RequestObject<byte[]>
            {
                Params = dicparam,
                Data = SerializerObject.ProtoBufSerialize(data, Saltkey)
            };

            var responseData = await Post(url, objData, appid);
            return responseData;
        }

        private async Task<ResponseObject> Post(string url, RequestObject<byte[]> apiobject, string appid)
        {
            try
            {
                var byteData = SerializerObject.ProtoBufSerialize(apiobject, null);
                var webreq = (HttpWebRequest)WebRequest.Create(url);
                webreq.Method = "POST";
                webreq.Timeout = 900000;
                webreq.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = webreq.GetRequestStream();
                dataStream.Write(byteData, 0, byteData.Length);
                dataStream.Close();
                WebResponse response = webreq.GetResponse();

                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);
                response.Close();
                var responseData = ms.ToArray();

                return SerializerObject.ProtoBufDeserialize<ResponseObject>(responseData, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <author>Hung Tran</author>
        /// <typeparam name="TU">The type of the u.</typeparam>
        /// <param name="u">The request.</param>
        /// <param name="url">The URL.</param>
        /// <param name="dictionary">The dicparam.</param>
        /// <param name="appid">The appid.</param>
        /// <param name="userid">The userid.</param>
        /// <returns></returns>
        public ResponseObject Post<TU>(TU u, string url, Dictionary<string, string> dictionary, string appid, int userid)
        {
            if (!dictionary.ContainsKey("userid"))
                dictionary.Add("userid", userid.ToString(CultureInfo.InvariantCulture));
            if (!dictionary.ContainsKey("appid"))
                dictionary.Add("appid", appid);
            if (IsNoneEncrypt)
            {
                if (!dictionary.ContainsKey("noneEncrypt"))
                    dictionary.Add("noneEncrypt", "1");
            }
            else
            {
                if (!dictionary.ContainsKey("signature"))
                    dictionary.Add("signature", Signature);
            }
            var objData = new RequestObject<byte[]>
            {
                Params = dictionary,
                Data = SerializerObject.ProtoBufSerialize(u, Saltkey)
            };
            var byteData = SerializerObject.ProtoBufSerialize(objData, null);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 200000;
            request.ContentType = "application/x-www-form-urlencoded";
            // Get the request stream.
            var stream = request.GetRequestStream();
            // Write the data to the request stream
            stream.Write(byteData, 0, byteData.Length);
            // Close the Stream object.
            stream.Close();
            // Get the response.
            var response = request.GetResponse();
            // Get the stream containing content returned by the server.
            stream = response.GetResponseStream();
            var memoryStream = new MemoryStream();
            if (stream != null)
                stream.CopyTo(memoryStream);
            response.Close();
            byte[] bytes = memoryStream.ToArray();
            var responseObject = SerializerObject.ProtoBufDeserialize<ResponseObject>(bytes, null);

            return responseObject;
        }

        public ResponseObject MobilePost<TU>(TU u, string url, Dictionary<string, string> dictionary)
        {
            if (IsNoneEncrypt)
            {
                if (!dictionary.ContainsKey("noneEncrypt"))
                    dictionary.Add("noneEncrypt", "1");
            }

            var objData = new RequestObject<byte[]>
            {
                Params = dictionary,
                Data = SerializerObject.ProtoBufSerialize(u, Saltkey)
            };
            var byteData = SerializerObject.ProtoBufSerialize(objData, null);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 200000;
            request.ContentType = "application/x-www-form-urlencoded";
            // Get the request stream.
            var stream = request.GetRequestStream();
            // Write the data to the request stream
            stream.Write(byteData, 0, byteData.Length);
            // Close the Stream object.
            stream.Close();
            // Get the response.
            var response = request.GetResponse();
            // Get the stream containing content returned by the server.
            stream = response.GetResponseStream();
            var memoryStream = new MemoryStream();
            if (stream != null)
                stream.CopyTo(memoryStream);
            response.Close();
            byte[] bytes = memoryStream.ToArray();
            var responseObject = SerializerObject.ProtoBufDeserialize<ResponseObject>(bytes, null);

            return responseObject;
        }

        public ResponseObject Post(byte[] byteData, string url, Dictionary<string, string> dictionary, string appid, int userid)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 200000;
            request.ContentType = "application/x-www-form-urlencoded";
            // Get the request stream.
            var stream = request.GetRequestStream();
            // Write the data to the request stream
            stream.Write(byteData, 0, byteData.Length);
            // Close the Stream object.
            stream.Close();
            // Get the response.
            var response = request.GetResponse();
            // Get the stream containing content returned by the server.
            stream = response.GetResponseStream();
            var memoryStream = new MemoryStream();
            if (stream != null)
                stream.CopyTo(memoryStream);
            response.Close();
            byte[] bytes = memoryStream.ToArray();

            var responseObject = SerializerObject.ProtoBufDeserialize<ResponseObject>(bytes, null);

            return responseObject;
        }

        public byte[] PostGetByte(byte[] byteData, string url, Dictionary<string, string> dictionary, string appid, int userid)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 200000;
            request.ContentType = "application/x-www-form-urlencoded";
            // Get the request stream.
            var stream = request.GetRequestStream();
            // Write the data to the request stream
            stream.Write(byteData, 0, byteData.Length);
            // Close the Stream object.
            stream.Close();
            // Get the response.
            var response = request.GetResponse();
            // Get the stream containing content returned by the server.
            stream = response.GetResponseStream();
            var memoryStream = new MemoryStream();
            if (stream != null)
                stream.CopyTo(memoryStream);
            response.Close();
            byte[] bytes = memoryStream.ToArray();

            return bytes;
        }

        public string SetResponseMessage(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
