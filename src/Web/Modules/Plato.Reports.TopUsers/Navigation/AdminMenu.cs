﻿using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Reports.TopUsers.Navigation
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
                .Add(T["Reports"], int.MaxValue - 4, questions => questions
                    .IconCss("fal fa-chart-bar")
                    .Add(T["Top Users"], 4, home => home
                        .Action("Index", "Admin", "Plato.Reports.TopUsers")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav())
                );

        }

    }

}
