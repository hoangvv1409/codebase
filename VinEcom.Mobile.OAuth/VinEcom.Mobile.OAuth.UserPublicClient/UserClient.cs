using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.OnePointApi.CVLibPublic;
using BusinessObjectPublicUser;

namespace VinEcom.Mobile.OAuth.UserPublicClient
{
    public interface IUserClient : IBasicClient
    {
        UserInfoResponse Login(UserLoginRequest request, out int status, out string message);

        UserInfoResponse Register(UserLoginRequest request, out int status, out string message);

        UserInfoResponse UpdateUserInfo(UserInfoRequest request, out int status, out string message);

        bool? ForgotPassword(UserInfoRequest request, out int status, out string message);

        bool? ChangePassword(ChangePasswordRequest request, out int status, out string message);

        TransactionInfoResponse GetTransactionInfo(TransactionInfoRequest request, out int status, out string message);

        UserCreditResponse GetUserCredit(UserInfoRequest request, out int status, out string message);
        bool? ValidOtpCode(ConfirmMobileRequest request, out int status, out string message);

        VingroupCardResponse GetListVingroupcardByUserId(int userId);
        bool ChangeCardToMain(int userId, string card_numbers);
        UserCardInfoResponse InsertUserCard(int userId, string card_numbers, bool setMainCard, string otp, string accountId, string fullName);
        UserCardInfoResponse GetCardInfo(int userId, string cardNumber, bool isVerify = false);
        bool ResendOtp(string mobileNumber, string cardNumber);
        bool RemoveCard(int userId, string cardNumber);
        bool VerifyCardInfo(string cardInfo, string idPassport, string mobile);
        bool VerifyOTPViaMobile(int userId, string cardNumber, string id, string mobileNumber);
        bool ForgotChangePassword(ChangePasswordRequest request);
    }

    public class UserClient : BasicClient, IUserClient
    {

        public UserInfoResponse Login(UserLoginRequest request, out int status, out string message)
        {
            var response = new UserInfoResponse();
            status = 0;
            message = string.Empty;
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.Login);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<UserInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public UserInfoResponse Register(UserLoginRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            var response = new UserInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.Register);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<UserInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public UserInfoResponse UpdateUserInfo(UserInfoRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            var response = new UserInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.UpdateUserInfo);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<UserInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public bool? ForgotPassword(UserInfoRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            bool? response = null;
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ForgotPassword);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public bool? ChangePassword(ChangePasswordRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            bool? response = null;
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ChangePassword);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public TransactionInfoResponse GetTransactionInfo(TransactionInfoRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            var response = new TransactionInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.GetTransactionInfo);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<TransactionInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public UserCreditResponse GetUserCredit(UserInfoRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            var response = new TransactionInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.GetUserCredit);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<UserCreditResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public bool? ValidOtpCode(ConfirmMobileRequest request, out int status, out string message)
        {
            status = 0;
            message = string.Empty;
            var response = new TransactionInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ValidOtpCode);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    status = result.Status;
                    message = result.Message;
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<bool?>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                status = 0;
                message = "Lỗi từ user api";
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public VingroupCardResponse GetListVingroupcardByUserId(int userId)
        {
            var response = new VingroupCardResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    UserId = userId
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.GetListVingroupCardByUserId);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<VingroupCardResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public bool ChangeCardToMain(int userId, string card_numbers)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    UserId = userId,
                    CardNumber = card_numbers
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ChangeToMainCard);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return false;
            }
        }

        public UserCardInfoResponse InsertUserCard(int userId, string card_numbers, bool setMainCard, string otp, string accountId, string fullName)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    UserId = userId,
                    CardNumber = card_numbers,
                    SetMainCard = setMainCard,
                    OTP = otp,
                    Csn = accountId,
                    FullName = fullName
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.InsertUserCard);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<UserCardInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return null;
            }
        }

        public UserCardInfoResponse GetCardInfo(int userId, string cardNumber, bool isVerify = false)
        {
            var response = new UserCardInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    UserId = userId,
                    CardNumber = cardNumber,
                    IsVerify = isVerify
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.GetCardInfo);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<UserCardInfoResponse>(result.ResponseData, client.Saltkey);
                }
                return response;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return default(UserCardInfoResponse);
            }
        }

        public bool ResendOtp(string mobileNumber, string cardNumber)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    EmailOrMobile = mobileNumber,
                    CardNumber = cardNumber
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ResendOTP);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return false;
            }
        }

        public bool RemoveCard(int userId, string cardNumber)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    UserId = userId,
                    CardNumber = cardNumber
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.RemoveCard);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return false;
            }
        }

        public bool VerifyCardInfo(string cardInfo, string idPassport, string mobile)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {
                    CardNumber = cardInfo,
                    Identity = idPassport,
                    EmailOrMobile = mobile
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.VerifyCardInfo);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return false;
            }
        }

        public bool VerifyOTPViaMobile(int userId, string cardNumber, string id, string mobileNumber)
        {
            var response = new TransactionInfoResponse();
            try
            {
                var request = new UserInfoRequest()
                {

                    UserId = userId,
                    CardNumber = cardNumber,
                    Identity = id,
                    EmailOrMobile = mobileNumber
                };
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.VerifyOTPViaMobile);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    Status = result.Status;
                    ResponseMessage = result.Message;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                Status = 0;
                ResponseMessage = SetResponseMessage(exception);
                return false;
            }
        }

        public bool ForgotChangePassword(ChangePasswordRequest request)
        {
            var response = new TransactionInfoResponse();
            try
            {
                IHttpOrderClient client = new HttpOrderClient(Publickey, true);
                var dic = new Dictionary<string, string>();
                var url = GetUrl(BusinessObjectPublicUser.RequestFunction.ForgotChangePassword);
                var result = client.Post(request, url, dic, Appid, Uid);

                if (result != null && result.ResponseData != null)
                {
                    ResponseMessage = result.Message;
                    Status = result.Status;
                    return SerializerObject.ProtoBufDeserialize<bool>(result.ResponseData, client.Saltkey);
                }
                return false;
            }
            catch (Exception exception)
            {
                ResponseMessage = "User API - " + SetResponseMessage(exception);
                return false;
            }
        }
    }
}
