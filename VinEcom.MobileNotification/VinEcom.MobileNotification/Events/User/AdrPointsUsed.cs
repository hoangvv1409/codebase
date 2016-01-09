﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Events.User
{
    public class AdrPointsUsed : IEvent
    {
        public int UserId { get; set; }
        public decimal AdrPoints { get; set; }

        public UserState UserState
        {
            get
            {
                return UserState.AdrPointsUsed;
            }
        }
    }
}
