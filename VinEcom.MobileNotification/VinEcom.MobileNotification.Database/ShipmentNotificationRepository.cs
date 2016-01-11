﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database
{
    public class ShipmentNotificationRepository : Repository<ShipmentNotification>
    {
        public ShipmentNotificationRepository(Func<MobileNotificationDbContext> dbContextFactory)
            : base(dbContextFactory)
        { }
    }
}