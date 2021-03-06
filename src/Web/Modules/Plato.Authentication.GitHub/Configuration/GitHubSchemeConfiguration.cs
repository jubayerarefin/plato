﻿using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OAuth.GitHub;
using Microsoft.Extensions.Options;
using Plato.GitHub.Models;

namespace Plato.Authentication.GitHub.Configuration
{

    public class GitHubSchemeConfiguration :
        IConfigureOptions<AuthenticationOptions>,
        IConfigureNamedOptions<GitHubAuthenticationOptions>
    {

        private readonly PlatoGitHubOptions _platoGitHubOptions;

        public GitHubSchemeConfiguration(
            IOptions<PlatoGitHubOptions> platoGitHubOptions)
        {
            _platoGitHubOptions = platoGitHubOptions.Value;
        }

        public void Configure(AuthenticationOptions options)
        {
            
            if (!ValidSettings())
            {
                return;
            }

            options.AddScheme(GitHubAuthenticationDefaults.AuthenticationScheme, builder =>
            {
                builder.DisplayName = "GitHub";
                builder.HandlerType = typeof(GitHubAuthenticationHandler);                
            });

        }

        public void Configure(GitHubAuthenticationOptions options) =>
            Configure(GitHubAuthenticationDefaults.AuthenticationScheme, options);

        public void Configure(string name, GitHubAuthenticationOptions options)
        {

            if (!string.Equals(name, GitHubAuthenticationDefaults.AuthenticationScheme, StringComparison.Ordinal))
            {
                return;
            }

            if (!ValidSettings())
            {
                return;
            }

            options.ClientId = _platoGitHubOptions.ClientId;
            options.ClientSecret = _platoGitHubOptions.ClientSecret;

            if (_platoGitHubOptions.CallbackPath.HasValue)
            {
                options.CallbackPath = _platoGitHubOptions.CallbackPath;
            }

        }

        bool ValidSettings()
        {

            if (string.IsNullOrEmpty(_platoGitHubOptions.ClientId)
                || string.IsNullOrEmpty(_platoGitHubOptions.ClientSecret))
            {
                return false;
            }

            return true;

        }

    }

}
