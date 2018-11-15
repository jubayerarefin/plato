﻿using System;

namespace Plato.Internal.Models.Notifications
{
 
    public interface INotificationContext
    {
        INotification Notification { get; set; }

        IServiceProvider ServiceProvider { get; }

    }
    
    public class NotificationContext : INotificationContext
    {

        public INotification Notification { get; set; }

        public IServiceProvider ServiceProvider { get; }

        public NotificationContext(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

    }

}