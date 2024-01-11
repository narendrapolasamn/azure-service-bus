﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; set; }
        public SubscriptionInfo(Type handlerType) {
            HandlerType = handlerType;
        }

        public static  SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}
