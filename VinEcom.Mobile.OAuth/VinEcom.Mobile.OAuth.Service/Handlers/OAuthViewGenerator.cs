using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;
using ADR.Mobile.Infrastructure.Messaging.Handling;
using ADR.Mobile.Infrastructure.Redis;
using ADR.Mobile.Infrastructure.Serialization;
using Newtonsoft.Json;
using VinEcom.Mobile.Authen.Contracts.Events;
using VinEcom.Mobile.OAuth.Contracts.Commands;
using VinEcom.Mobile.OAuth.Contracts.Events;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.Commands;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Database.Repository;

namespace VinEcom.Mobile.OAuth.Service.Handlers
{
    public class OAuthViewGenerator :
        ICommandHandler<CreateUserDevice>,
        ICommandHandler<CreateRefreshToken>,
        ICommandHandler<ReduceRefreshTokenTTL>,
        ICommandHandler<RemoveRefreshToken>,
        ICommandHandler<ForceUserLogin>,
        IEventHandler<AppSecretChanged>
    {
        private IEventBus eventBus;

        private RedisWriteClient redisWriteClient;
        private RedisReadClient redisReadClient;

        private string RefreshTokenPrefixNamespace;
        private string UserDevicePrefixNamespace;

        private IUserDeviceRepository userDeviceRepository;

        public OAuthViewGenerator(RedisWriteClient redisWriteClient, RedisReadClient redisReadClient, IEventBus eventBus, IUserDeviceRepository userDeviceRepository)
        {
            this.eventBus = eventBus;

            this.redisWriteClient = redisWriteClient;
            this.redisReadClient = redisReadClient;

            this.RefreshTokenPrefixNamespace = "Ticket";
            this.UserDevicePrefixNamespace = "UserDevice";

            this.userDeviceRepository = userDeviceRepository;
        }

        public void Handle(CreateUserDevice command)
        {
            var userDevice = new UserDevice(command.UserId, command.ClientId);

            //Storage
            if (!this.userDeviceRepository.IsExist(command.UserId, command.ClientId))
            {
                this.userDeviceRepository.Save(userDevice);
            }

            //Redis
            var key = string.Format("{0}:{1}:{2}", this.UserDevicePrefixNamespace, command.UserId, command.ClientId);
            var value = JsonConvert.SerializeObject(userDevice);
            this.redisWriteClient.Set(key, value);

            //TODO Do we need persist before send Publish UserDeviceCreated then delete?
            //this.PublishEvent(new UserDeviceCreated()
            //{
            //    DeviceId = userDevice.ClientId,
            //    UserId = userDevice.UserId
            //});
        }

        public void Handle(CreateRefreshToken command)
        {
            var key = string.Format("{0}:{1}:{2}", this.RefreshTokenPrefixNamespace, command.ClientId, command.RefreshToken);

            this.redisWriteClient.Set(key, command.AuthenticationTicket, command.ExpiresIn);

            //TODO Do we need persist before send Publish RefreshTokenCreated then delete?
            //this.PublishEvent(new RefreshTokenCreated()
            //{
            //    DeviceId = command.ClientId,
            //    CreatedTime = command.CreatedTime,
            //    ExpiredTime = command.ExpiredTime
            //});
        }

        public void Handle(RemoveRefreshToken command)
        {
            var key = string.Format("{0}:{1}:{2}", this.RefreshTokenPrefixNamespace, command.ClientId, command.RefreshToken);

            this.redisWriteClient.Remove(key);

            //TODO Do we need persist before send Publish RefreshTokenRemoved then delete?
            //this.PublishEvent(new RefreshTokenRemoved()
            //{
            //    RefreshToken = command.RefreshToken
            //});
        }

        private void PublishEvent(IEvent @event)
        {
            this.eventBus.Publish(new Envelope<IEvent>(@event));
        }

        public void Handle(ReduceRefreshTokenTTL command)
        {
            var key = string.Format("{0}:{1}:{2}", this.RefreshTokenPrefixNamespace, command.ClientId, command.RefreshToken);

            this.redisWriteClient.Set(key, command.AuthenticationTicket, TimeSpan.FromMinutes(command.TTL));
        }

        //TODO Implement later
        public void Handle(AppSecretChanged @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(ForceUserLogin command)
        {
            var clientIds = this.userDeviceRepository.GetListClientId(command.UserId);
            foreach (var clientId in clientIds)
            {
                if (clientId == command.ClientId) continue;

                var key = string.Format("{0}:{1}:{2}", this.UserDevicePrefixNamespace, command.UserId, clientId);
                var userDevice = new UserDevice(command.UserId, clientId);
                userDevice.SetNeedLoginTo(true);
                var value = JsonConvert.SerializeObject(userDevice);

                this.redisWriteClient.Set(key, value);
            }
        }
    }
}
