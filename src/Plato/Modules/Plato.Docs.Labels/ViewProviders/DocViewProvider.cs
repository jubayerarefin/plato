﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Labels.Services;
using Plato.Docs.Models;
using Plato.Labels.ViewModels;
using Label = Plato.Docs.Labels.Models.Label;

namespace Plato.Docs.Labels.ViewProviders
{
    public class DocViewProvider : BaseViewProvider<Doc>
    {

        private const string LabelHtmlName = "label";
        
        private readonly IEntityLabelManager<EntityLabel> _entityLabelManager;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;
        private readonly ICacheManager _cacheManager;
        private readonly HttpRequest _request;

        private readonly IStringLocalizer T;

        public DocViewProvider(
            IEntityLabelManager<EntityLabel> entityLabelManager,
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Doc> entityStore,
            ILabelStore<Label> labelStore,
            IStringLocalizer stringLocalize,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade,
            ICacheManager cacheManager)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityLabelManager = entityLabelManager;
            _entityLabelStore = entityLabelStore;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _cacheManager = cacheManager;
            _entityStore = entityStore;
            _labelStore = labelStore;

            T = stringLocalize;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Doc viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Doc viewModel, IViewProviderContext updater)
        {
            
            // Get entity labels
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    q.EntityId.Equals(viewModel.Id);
                })
                .OrderBy("Name", OrderBy.Desc)
                .ToList();
            
            return Views(
                View<LabelsViewModel<Label>>("Doc.Labels.Display.Sidebar", model =>
                {
                    model.Labels = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(3)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Doc topic, IViewProviderContext updater)
        {
            
            List<int> selectedLabels;
            // Persist state on post back
            if (_request.Method == "POST")
            {
                selectedLabels = await GetLabelsToAddAsync();
            }
            else
            {
                // Get entity labels
                var labels = await GetEntityLabelsByEntityIdAsync(topic.Id);
                selectedLabels = labels?.Select(l => l.LabelId).ToList();
            }
            
            var viewModel = new LabelDropDownViewModel()
            {
                Options = new LabelIndexOptions()
                {
                    FeatureId = await GetFeatureIdAsync()
                },
                HtmlName = LabelHtmlName,
                SelectedLabels = selectedLabels?.ToArray()
            };
            
            return Views(
                View<LabelDropDownViewModel>("Doc.Labels.Edit.Sidebar", model => viewModel).Zone("sidebar")
                    .Order(2)
            );

        }
        
        public override Task<bool> ValidateModelAsync(Doc topic, IUpdateModel updater)
        {
            // ensure labels are optional
            return Task.FromResult(true);
        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Doc doc, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(doc.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(doc, context);
            }

            // Validate model
            if (await ValidateModelAsync(doc, context.Updater))
            {

                // Get selected labels
                //var labelsToAdd = GetLabelsToAdd();
                var labelsToAdd = await GetLabelsToAddAsync();
                
                // Build labels to remove
                var labelsToRemove = new List<EntityLabel>();
                foreach (var entityLabel in await GetEntityLabelsByEntityIdAsync(doc.Id))
                {
                    // Entry already exists remove from labels to add
                    if (labelsToAdd.Contains(entityLabel.LabelId))
                    {
                        labelsToAdd.Remove(entityLabel.LabelId);
                    }
                    else
                    {
                        // Entry does NOT exist in labels to add ensure it's removed
                        labelsToRemove.Add(entityLabel);
                    }
                }

                // Remove entity labels
                foreach (var entityLabel in labelsToRemove)
                {
                    var result = await _entityLabelManager.DeleteAsync(entityLabel);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                // Get authenticated user
                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // Add new entity labels
                foreach (var labelId in labelsToAdd)
                {
                    var result = await _entityLabelManager.CreateAsync(new EntityLabel()
                    {
                        EntityId = doc.Id,
                        LabelId = labelId,
                        CreatedUserId = user?.Id ?? 0,
                        CreatedDate = DateTime.UtcNow
                    });
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    // Ensure we clear our labels cache to return new associations
                    _cacheManager.CancelTokens(typeof(LabelStore<Label>));

                }

            }

            return await BuildEditAsync(doc, context);

        }
        
        async Task<List<int>> GetLabelsToAddAsync()
        {
            // Build selected channels
            var labelsToAdd = new List<int>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.Equals(LabelHtmlName))
                {
                    var value = _request.Form[key];
                    if (!String.IsNullOrEmpty(value))
                    {
                        var items = JsonConvert.DeserializeObject<IEnumerable<LabelApiResult>>(value);
                        foreach (var item in items)
                        {
                            if (item.Id > 0)
                            {
                                var label = await _labelStore.GetByIdAsync(item.Id);
                                if (label != null)
                                {
                                    labelsToAdd.Add(label.Id);
                                }
                            }
                        }
                    }
                 
                }
            }

            return labelsToAdd;
        }
        
        async Task<IEnumerable<EntityLabel>> GetEntityLabelsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return new List<EntityLabel>();
            }

            return await _entityLabelStore.GetByEntityIdAsync(entityId) ?? new List<EntityLabel>();

        }
        
        async Task<int> GetFeatureIdAsync()
        {
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
            if (feature != null)
            {
                return feature.Id;
            }

            throw new Exception($"Could not find required feature registration for Plato.Docs");
        }
        
    }

}