﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Models.Users
{
    public class UserAvatar
    {

        public string Url { get; set; }

        public UserAvatar(ISimpleUser user)
        {

            // If we have a photo Url use that
            if (!string.IsNullOrEmpty(user.PhotoUrl))
            {
                Url = user.PhotoUrl;
                return;
            }

            // Else fallback to our letter service
            Url = $"/users/letter/{user.DisplayName.Substring(0, 1)}/{user.PhotoColor}";

        }

    }
}