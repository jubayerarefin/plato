﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Entities.Services;
using PlatoCore.Security.Abstractions;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Questions.ViewComponents
{

    public class QuestionListViewComponent : ViewComponentBase
    {

        private readonly ICollection<Filter> _defaultFilters = new List<Filter>()
        {
            new Filter()
            {
                Text = "All",
                Value = FilterBy.All
            },
            new Filter()
            {
                Text = "-" // represents menu divider
            },
            new Filter()
            {
                Text = "Started",
                Value = FilterBy.Started
            },
            new Filter()
            {
                Text = "Participated",
                Value = FilterBy.Participated
            },
            new Filter()
            {
                Text = "Following",
                Value = FilterBy.Following
            },
            new Filter()
            {
                Text = "Starred",
                Value = FilterBy.Starred
            },
            new Filter()
            {
                Text = "-"  // represents menu divider
            },
            new Filter()
            {
                Text = "Unanswered",
                Value = FilterBy.Unanswered
            },
            new Filter()
            {
                Text = "No Replies",
                Value = FilterBy.NoReplies
            }
        };

        private readonly ICollection<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Latest",
                Value = SortBy.LastReply
            },
            new SortColumn()
            {
                Text = "Popular",
                Value = SortBy.Popular
            },
            new SortColumn()
            {
                Text = "-" // represents menu divider
            },
            new SortColumn()
            {
                Text = "Answers",
                Value =  SortBy.Replies
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "Participants",
                Value =  SortBy.Participants
            },
            new SortColumn()
            {
                Text = "Reactions",
                Value =  SortBy.Reactions
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Stars",
                Value =  SortBy.Stars
            },
            new SortColumn()
            {
                Text = "Created",
                Value = SortBy.Created
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly ICollection<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };        
        
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityService<Question> _articleService;

        public QuestionListViewComponent(            
            IAuthorizationService authorizationService,
            IEntityService<Question> articleService)
        {
            _authorizationService = authorizationService;
            _articleService = articleService;            
        }

        public async Task<IViewComponentResult> InvokeAsync(EntityIndexViewModel<Question> model)
        {

            if (model == null)
            {
                model = new EntityIndexViewModel<Question>();
            }

            if (model.Options == null)
            {
                model.Options = new EntityIndexOptions();
            }

            if (model.Pager == null)
            {
                model.Pager = new PagerOptions();
            }
            
            return View(await GetViewModel(model));

        }
        
        async Task<EntityIndexViewModel<Question>> GetViewModel(EntityIndexViewModel<Question> model)
        {
            
            // Get results
            var results = await _articleService
                .ConfigureQuery(async q =>
                {
                    
                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateQuestions))
                    {
                        q.HidePrivate.True();
                    }
                  
                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenQuestions))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamQuestions))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedQuestions))
                    {
                        q.HideDeleted.True();
                    }
                    
                })
                .GetResultsAsync(model.Options, model.Pager);

            // Set total on pager
            model.Pager.SetTotal(results?.Total ?? 0);

            model.SortColumns = _defaultSortColumns;
            model.SortOrder = _defaultSortOrder;
            model.Filters = _defaultFilters;
            model.Results = results;

            return model;

        }

    }
    
}

