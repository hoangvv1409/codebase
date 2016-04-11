using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;

namespace VinEcom.Mobile.OAuth.Contracts.Commands
{
    public class CreateUserDevice : ICommand
    {
        public CreateUserDevice()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public long UserId { get; set; }
    }
}
