﻿using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Search.Navigation
{

    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Settings"], int.MaxValue, s => s
                    .IconCss("fal fa-cog")
                    .Add(T["Search"], int.MinValue + 50, installed => installed
                        .Action("Index", "Admin", "Plato.Search")
                        .Permission(Permissions.ManageSearchSettings)
                        .LocalNav()
                    ));

        }

    }

}
