﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Database
{
    public class OrderNotificationRepository : Repository<OrderNotification>
    {
        public OrderNotificationRepository(Func<MobileNotificationDbContext> dbContextFactory)
            : base(dbContextFactory)
        { }


    }
}
