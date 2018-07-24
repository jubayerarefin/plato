﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Hosting.Web.Configuration
{
    public class AntiForgeryOptionsConfiguration : IConfigureOptions<AntiforgeryOptions>
    {

        public AntiForgeryOptionsConfiguration()
        {
        }

        public void Configure(AntiforgeryOptions options)
        {
            options.Cookie.Name = "plato_antiforgery";
            options.HeaderName = "X-Csrf-Token";
            options.FormFieldName = "plato-csrf";
            options.SuppressXFrameOptionsHeader = false;
        }
        
    }
}
