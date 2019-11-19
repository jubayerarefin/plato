﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Plato.Google.Models;
using Plato.Google.Stores;
using Plato.Google.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Google.Configuration;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;

namespace Plato.Google.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<PlatoGoogleSettings>
    {

        private readonly IGoogleSettingsStore<PlatoGoogleSettings> _googleSettingsStore;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILogger<AdminViewProvider> _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        private readonly PlatoOptions _platoOptions;

        public AdminViewProvider(
            IGoogleSettingsStore<PlatoGoogleSettings> googleSettingsStore,
            IDataProtectionProvider dataProtectionProvider,
            IOptions<PlatoOptions> platoOptionsAccessor,
            ILogger<AdminViewProvider> logger,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _googleSettingsStore = googleSettingsStore;
            _platoOptions = platoOptionsAccessor.Value;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
            _logger = logger;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(PlatoGoogleSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(PlatoGoogleSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(PlatoGoogleSettings settings, IViewProviderContext context)
        {
            var viewModel = await GetModel();
            return Views(
                View<GoogleSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<GoogleSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(PlatoGoogleSettings settings, IViewProviderContext context)
        {

            var model = new GoogleSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {

                // Encrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(model.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoGoogleOptionsConfiguration));
                        secret = protector.Protect(model.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem encrypting the google client secret. {e.Message}");
                    }
                }

                // Create the model
                settings = new PlatoGoogleSettings()
                {
                    ClientId = model.ClientId,
                    ClientSecret = secret,
                    CallbackPath = model.CallbackPath,
                    TrackingId = model.TrackingId
                };

                // Persist the settings
                var result = await _googleSettingsStore.SaveAsync(settings);
                if (result != null)
                {
                    // Recycle shell context to ensure changes take effect
                    _platoHost.RecycleShellContext(_shellSettings);
                }

            }

            return await BuildEditAsync(settings, context);

        }

        async Task<GoogleSettingsViewModel> GetModel()
        {

            var settings = await _googleSettingsStore.GetAsync();
            if (settings != null)
            {

                // Decrypt the secret
                var secret = string.Empty;
                if (!string.IsNullOrWhiteSpace(settings.ClientSecret))
                {
                    try
                    {
                        var protector = _dataProtectionProvider.CreateProtector(nameof(PlatoGoogleOptionsConfiguration));
                        secret = protector.Unprotect(settings.ClientSecret);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was a problem decrypting the google client secret. {e.Message}");
                    }
                }

                return new GoogleSettingsViewModel()
                {
                    ClientId = _platoOptions.DemoMode ? "123456789" : settings.ClientId,
                    ClientSecret = _platoOptions.DemoMode ? "xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx" : secret,
                    CallbackPath = _platoOptions.DemoMode ? string.Empty : settings.CallbackPath.ToString(),
                    TrackingId = _platoOptions.DemoMode ? "UA-123456789-0" : settings.TrackingId
                };

            }

            // return default settings
            return new GoogleSettingsViewModel()
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty,
                CallbackPath = string.Empty,
                TrackingId  = string.Empty
            };

        }

    }

}
