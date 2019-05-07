﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.ViewComponents
{

    public class GetIdeaCommentListViewComponent : ViewComponent
    {
        
        private readonly IEntityReplyService<IdeaComment> _replyService;
        private readonly IEntityStore<Idea> _entityStore;
        private readonly IAuthorizationService _authorizationService;

        public GetIdeaCommentListViewComponent(
            IEntityReplyService<IdeaComment> replyService,
            IEntityStore<Idea> entityStore,
            IAuthorizationService authorizationService)
        {
            _replyService = replyService;
            _entityStore = entityStore;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<EntityViewModel<Idea, IdeaComment>> GetViewModel(EntityOptions options, PagerOptions pager)
        {
            
            var topic = await _entityStore.GetByIdAsync(options.Id);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateIdeaComments))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamIdeaComments))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedIdeaComment))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityViewModel<Idea, IdeaComment>
            {
                Options = options,
                Pager = pager,
                Entity = topic,
                Replies = results
            };

        }

    }

}



