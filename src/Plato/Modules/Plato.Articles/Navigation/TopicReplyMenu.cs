﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Navigation
{

    public class TopicReplyMenu : INavigationProvider
    {

        private readonly IActionContextAccessor _actionContextAccessor;
    
        public IStringLocalizer T { get; set; }

        public TopicReplyMenu(
            IStringLocalizer localizer,
            IActionContextAccessor actionContextAccessor)
        {
            T = localizer;
            _actionContextAccessor = actionContextAccessor;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "topic-reply", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get topic from context
            var topic = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            if (topic == null)
            {
                return;
            }
            
            // Get reply from context
            var reply = builder.ActionContext.HttpContext.Items[typeof(ArticleComment)] as ArticleComment;
            if (reply == null)
            {
                return;
            }

            //// Get authenticated user from features
            var user = builder.ActionContext.HttpContext.Features[typeof(User)] as User;

            // Get delete / restore permission
            Permission deletePermission = null;
            if (reply.IsDeleted)
            {
                deletePermission = user?.Id == reply.CreatedUserId
                    ? Permissions.RestoreOwnReplies
                    : Permissions.RestoreAnyReply;
            }
            else
            {
                deletePermission = user?.Id == reply.CreatedUserId
                    ? Permissions.DeleteOwnReplies
                    : Permissions.DeleteAnyReply;
            }
            
            // Options
            builder
                .Add(T["Options"], int.MaxValue, options => options
                        .IconCss("fa fa-ellipsis-h")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Options"]}
                        })
                        .Add(T["Edit"], int.MinValue, edit => edit
                            .Action("EditReply", "Home", "Plato.Discuss", new RouteValueDictionary()
                            {
                                ["id"] = reply?.Id ?? 0
                            })
                            .Permission(user?.Id == reply.CreatedUserId ?
                                Permissions.EditOwnReplies :
                                Permissions.EditAnyReply)
                            .LocalNav())
                        .Add(T["Report"], int.MaxValue - 10, report => report
                            .Action("Report", "Home", "Plato.Discuss")
                            .Attributes(new Dictionary<string, object>()
                            {
                                {"data-provide", "dialog"},
                                {"data-dialog-modal-css", "modal fade"},
                                {"data-dialog-css", "modal-dialog modal-lg"}
                            })
                            .Permission(Permissions.ReportReplies)
                            .LocalNav()
                        )
                        .Add(reply.IsDeleted ? T["Restore"] : T["Delete"], int.MaxValue, edit => edit
                                .Action(reply.IsDeleted ? "RestoreReply" : "DeleteReply", "Home", "Plato.Discuss",
                                    new RouteValueDictionary()
                                    {
                                        ["id"] = reply.Id
                                    })
                                .Permission(deletePermission)
                                .LocalNav(),
                            reply.IsDeleted
                                ? new List<string>() { "dropdown-item", "dropdown-item-success" }
                                : new List<string>() { "dropdown-item", "dropdown-item-danger" }
                        )
                    , new List<string>() {"topic-options", "text-muted", "dropdown-toggle-no-caret", "text-hidden"}
                );

            if (!topic.IsClosed)
            {
                builder
                    .Add(T["Reply"], int.MaxValue, options => options
                            .IconCss("fa fa-reply")
                            .Attributes(user == null
                                ? new Dictionary<string, object>()
                                {
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Login to Reply"]}
                                }
                                : new Dictionary<string, object>()
                                {
                                    {"data-provide", "postQuote"},
                                    {"data-quote-selector", "#quote" + reply.Id.ToString()},
                                    {"data-toggle", "tooltip"},
                                    {"title", T["Reply"]}
                                })
                            .Action("Login", "Account", "Plato.Users",
                                new RouteValueDictionary()
                                {
                                    ["returnUrl"] = builder.ActionContext.HttpContext.Request.Path
                                })
                            .Permission(Permissions.PostReplies)
                            .LocalNav()
                        , new List<string>() { "topic-reply", "text-muted", "text-hidden" }
                    );

            }



        }

    }

}
