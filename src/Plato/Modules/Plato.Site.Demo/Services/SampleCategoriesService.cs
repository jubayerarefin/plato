﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Site.Demo.Models;
using Plato.Users.Services;

namespace Plato.Site.Demo.Services
{
  
    public class SampleCategoriesService : ISampleCategoriesService
    {

        Random _random;

        private readonly List<SampleDataDescriptor> Descriptors = new List<SampleDataDescriptor>()
        {
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Discuss.Categories",
                EntityType = "topic"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Docs.Categories",
                EntityType = "doc",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Articles.Categories",
                EntityType = "articles",
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Ideas.Categories",
                EntityType = "idea"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Issues.Categories",
                EntityType = "issue"
            },
            new SampleDataDescriptor()
            {
                ModuleId = "Plato.Questions.Categories",
                EntityType = "question"
            }
        };

        private readonly ICategoryManager<CategoryBase> _categoryManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IUserColorProvider _colorProvider;
        private readonly IDbHelper _dbHelper;

        public SampleCategoriesService(
            ICategoryManager<CategoryBase> categoryManager,
            IUserColorProvider colorProvider,
            IFeatureFacade featureFacade,             
             IDbHelper dbHelper)
        {
            _categoryManager = categoryManager;
            _featureFacade = featureFacade;
            _colorProvider = colorProvider;
            _dbHelper = dbHelper;
            _random = new Random();
        }

        public async Task<ICommandResultBase> InstallAsync()
        {
            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {
                var result = await InstallCategoriesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();

        }

        public async Task<ICommandResultBase> UninstallAsync()
        {

            var output = new CommandResultBase();
            foreach (var descriptor in Descriptors)
            {
                var result = await UninstallCategoriesAsync(descriptor);
                if (!result.Succeeded)
                {
                    return output.Failed(result.Errors.ToArray());
                }
            }

            return output.Success();
        }

        // ----------

        async Task<ICommandResultBase> InstallCategoriesAsync(SampleDataDescriptor descriptor)
        {

            // Validate

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.ModuleId))
            {
                throw new ArgumentNullException(nameof(descriptor.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            for (var i = 0; i <= 10; i++)
            {
                var categoryResult = await InstallCategoryAsync(descriptor, i);
                if (!categoryResult.Succeeded)
                {
                    return result.Failed(categoryResult.Errors.ToArray());
                }
            }

            return result.Success();

        }

        async Task<ICommandResultBase> InstallCategoryAsync(SampleDataDescriptor descriptor, int sortOrder = 0)
        {

            // Validate

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.ModuleId))
            {
                throw new ArgumentNullException(nameof(descriptor.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);

            if (feature == null)
            {
                return result.Failed($"The feature {descriptor.ModuleId} is not enabled!");
            }

            var icons = new DefaultIcons();

            var foreColor = "#ffffff";
            var backColor = $"#{_colorProvider.GetColor()}";
            var iconCss = $"fal fa-{icons.GetIcon()}";
 
            var categoryResult = await _categoryManager.CreateAsync(new CategoryBase()
            {
                FeatureId = feature.Id,
                ParentId = 0,
                Name = $"Example Category {_random.Next(0, 2000).ToString()}",
                Description = $"This is just an example category desccription.",
                ForeColor = foreColor,
                BackColor = backColor,
                IconCss = iconCss,
                SortOrder = sortOrder
            });

            if (!categoryResult.Succeeded)
            {
                return result.Failed(result.Errors.ToArray());
            }

            return result.Success();

        }

        async Task<ICommandResultBase> UninstallCategoriesAsync(SampleDataDescriptor descriptor)
        {

            // Validate

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.ModuleId))
            {
                throw new ArgumentNullException(nameof(descriptor.ModuleId));
            }

            // Our result
            var result = new CommandResultBase();

            // Ensure the feature is enabled
            var feature = await _featureFacade.GetFeatureByIdAsync(descriptor.ModuleId);

            if (feature == null)
            {
                return result.Failed($"The feature {descriptor.ModuleId} is not enabled!");
            }

            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{featureId}"] = feature.Id.ToString()
            };

            // Sql to execute
            var sql = @"
                DELETE FROM {prefix}_CategoryData WHERE CategoryId IN (
                    SELECT Id FROM {prefix}_Categories WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_CategoryRoles WHERE CategoryId IN (
                    SELECT Id FROM {prefix}_Categories WHERE FeatureId = {featureId}
                );
                DELETE FROM {prefix}_Categories WHERE FeatureId = {featureId};
            ";

            // Execute and return result
            var error = string.Empty;
            try
            {
                await _dbHelper.ExecuteScalarAsync<int>(sql, replacements);
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return !string.IsNullOrEmpty(error)
                ? result.Failed(error)
                : result.Success();

        }

    }

}
