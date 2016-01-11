using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.CoreServices;
using VinEcom.MobileNotification.Database;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Service.Handlers
{
    public class UserEventHandler :
        IEventHandler<AdrPointsUsed>,
        IEventHandler<AdrPointAdded>,
        IEventHandler<AdrPointsRefunded>,
        IEventHandler<EmailAdded>,
        IEventHandler<MobileAdded>,
        IEventHandler<PasswordChanged>,
        IEventHandler<TwoStepsActivated>
    {
        private IContentGenerator contentGenerator;
        private Func<MobileNotificationDbContext> dbContext;

        public UserEventHandler(Func<MobileNotificationDbContext> dbContext, IContentGenerator contentGenerator)
        {
            this.contentGenerator = contentGenerator;
            this.dbContext = dbContext;
        }

        public void Handle(AdrPointsUsed e)
        {
            GenerateContent(UserState.AdrPointsUsed, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content,
                SOID = e.SOID
            });
        }

        public void Handle(AdrPointAdded e)
        {
            GenerateContent(UserState.AdrPointAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(AdrPointsRefunded e)
        {
            GenerateContent(UserState.AdrPointsRefunded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content,
                SOID = e.SOID
            });
        }

        public void Handle(EmailAdded e)
        {
            GenerateContent(UserState.EmailAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(MobileAdded e)
        {
            GenerateContent(UserState.MobileAdded, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(PasswordChanged e)
        {
            GenerateContent(UserState.PasswordChanged, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        public void Handle(TwoStepsActivated e)
        {
            GenerateContent(UserState.TwoStepsActivated, e, (content) => new MobileMessage()
            {
                UserId = e.UserId,
                Title = "Adayroi.com",
                Message = content
            });
        }

        private void GenerateContent(UserState state, IEvent e, Func<string, MobileMessage> bindingFunction)
        {
            string template = this.GetTemplate(state);
            string content = contentGenerator.Generate(template, e);

            MobileMessage mobileMessage = bindingFunction.Invoke(content);

            this.SaveMobileMessage(mobileMessage);
        }

        private string GetTemplate(UserState state)
        {
            MessageTemplateRepository repository = new MessageTemplateRepository(dbContext);

            return repository.GetTemplateByStateAndResourceType((int)ResourceTypes.User, (int)state);
        }

        private void SaveMobileMessage(MobileMessage mobileMessage)
        {
            MobileMessageRepository repository = new MobileMessageRepository(dbContext);

            repository.Save(mobileMessage);
            repository.SaveChanges();
        }
    }
}
