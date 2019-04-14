﻿using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostQuestions =
            new Permission("PostQuestions", "Post questions");

        public static readonly Permission PostAnswers =
            new Permission("PostAnswers", "Post answers");

        public static readonly Permission EditOwnQuestions =
            new Permission("EditOwnQuestions", "Edit own questions");

        public static readonly Permission EditAnyQuestion =
            new Permission("EditAnyQuestion", "Edit any questions");
        
        public static readonly Permission EditOwnAnswers =
            new Permission("EditOwnAnswers", "Edit own answers");

        public static readonly Permission EditAnyAnswer =
            new Permission("EditAnyAnswer", "Edit any answer");
        
        public static readonly Permission DeleteOwnQuestions = 
            new Permission("DeleteOwnQuestions", "Delete own questions");

        public static readonly Permission RestoreOwnQuestions =
            new Permission("RestoreOwnQuestions", "Restore own questions");
        
        public static readonly Permission DeleteAnyQuestion =
            new Permission("DeleteAnyQuestion", "Delete any question");

        public static readonly Permission RestoreAnyQuestion =
            new Permission("RestoreAnyQuestion", "Restore any question");
        
        public static readonly Permission DeleteOwnAnswers =
            new Permission("DeleteOwnAnswers", "Delete own answers");

        public static readonly Permission RestoreOwnAnswers =
            new Permission("RestoreOwnAnswers", "Restore own answers");
        
        public static readonly Permission DeleteAnyAnswer =
            new Permission("DeleteAnyAnswer", "Delete any answer");

        public static readonly Permission RestoreAnyAnswer =
            new Permission("RestoreAnyAnswer", "Restore any answer");

        public static readonly Permission ReportQuestions =
            new Permission("ReportQuestions", "Report questions");

        public static readonly Permission ReportAnswers =
            new Permission("ReportAnswers", "Report answers");
        
        public static readonly Permission ViewDeletedQuestions =
            new Permission("ViewDeletedQuestions", "View deleted questions");

        public static readonly Permission ViewDeletedAnswers =
            new Permission("ViewDeletedAnswers", "View deleted answers");
        
        public static readonly Permission PinQuestions =
            new Permission("PinQuestions", "Pin questions");

        public static readonly Permission UnpinQuestions =
            new Permission("UnpinQuestions", "Unpin questions");

        public static readonly Permission LockQuestions =
            new Permission("LockQuestions", "Lock questions");

        public static readonly Permission UnlockQuestions =
            new Permission("UnlockQuestions", "Unlock questions");

        public static readonly Permission HideQuestions =
            new Permission("HideQuestions", "Hide questions");

        public static readonly Permission ShowQuestions =
            new Permission("ShowQuestions", "Unhide questions");

        public static readonly Permission ViewPrivateQuestions =
            new Permission("ViewPrivateQuestions", "View hidden questions");

        public static readonly Permission HideAnswers =
            new Permission("HideAnswers", "Hide answers");

        public static readonly Permission ShowAnswers =
            new Permission("ShowAnswers", "Unhide answers");

        public static readonly Permission ViewPrivateAnswers =
            new Permission("ViewPrivateAnswers", "View hidden answers");

        public static readonly Permission QuestionToSpam =
            new Permission("QuestionToSpam", "Move questions to SPAM");

        public static readonly Permission QuestionFromSpam =
            new Permission("QuestionFromSpam", "Remove questions from SPAM");

        public static readonly Permission ViewSpamQuestions =
            new Permission("ViewSpamQuestions", "View questions flagged as SPAM");

        public static readonly Permission AnswerToSpam =
            new Permission("AnswerToSpam", "Move answers to SPAM");

        public static readonly Permission AnswerFromSpam =
            new Permission("AnswerFromSpam", "Remove answers from SPAM");

        public static readonly Permission ViewSpamAnswers =
            new Permission("ViewSpamAnswers", "View answers flagged as SPAM");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostQuestions,
                PostAnswers,
                EditOwnQuestions,
                EditAnyQuestion,
                EditOwnAnswers,
                EditAnyAnswer,
                DeleteOwnQuestions,
                RestoreOwnQuestions,
                DeleteAnyQuestion,
                RestoreAnyQuestion,
                ViewDeletedQuestions,
                DeleteOwnAnswers,
                RestoreOwnAnswers,
                DeleteAnyAnswer,
                RestoreAnyAnswer,
                ViewDeletedAnswers,
                ReportQuestions,
                ReportAnswers,
                PinQuestions,
                UnpinQuestions,
                LockQuestions,
                UnlockQuestions,
                HideQuestions,
                ShowQuestions,
                ViewPrivateQuestions,
                HideAnswers,
                ShowAnswers,
                ViewPrivateAnswers,
                QuestionToSpam,
                QuestionFromSpam,
                ViewSpamQuestions,
                AnswerToSpam,
                AnswerFromSpam,
                ViewSpamAnswers
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditAnyQuestion,
                        EditOwnAnswers,
                        EditAnyAnswer,
                        DeleteOwnQuestions,
                        RestoreOwnQuestions,
                        DeleteAnyQuestion,
                        RestoreAnyQuestion,
                        ViewDeletedQuestions,
                        DeleteOwnAnswers,
                        RestoreOwnAnswers,
                        DeleteAnyAnswer,
                        RestoreAnyAnswer,
                        ViewDeletedAnswers,
                        ReportQuestions,
                        ReportAnswers,
                        PinQuestions,
                        UnpinQuestions,
                        LockQuestions,
                        UnlockQuestions,
                        HideQuestions,
                        ShowQuestions,
                        ViewPrivateQuestions,
                        HideAnswers,
                        ShowAnswers,
                        ViewPrivateAnswers,
                        QuestionToSpam,
                        QuestionFromSpam,
                        ViewSpamQuestions,
                        AnswerToSpam,
                        AnswerFromSpam,
                        ViewSpamAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditOwnAnswers,
                        DeleteOwnAnswers,
                        ReportQuestions,
                        ReportAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostQuestions,
                        PostAnswers,
                        EditOwnQuestions,
                        EditAnyQuestion,
                        EditOwnAnswers,
                        EditAnyAnswer,
                        DeleteOwnQuestions,
                        RestoreOwnQuestions,
                        DeleteAnyQuestion,
                        RestoreAnyQuestion,
                        ViewDeletedQuestions,
                        DeleteOwnAnswers,
                        RestoreOwnAnswers,
                        DeleteAnyAnswer,
                        RestoreAnyAnswer,
                        ViewDeletedAnswers,
                        ReportQuestions,
                        ReportAnswers,
                        PinQuestions,
                        UnpinQuestions,
                        LockQuestions,
                        UnlockQuestions,
                        HideQuestions,
                        ShowQuestions,
                        ViewPrivateQuestions,
                        HideAnswers,
                        ShowAnswers,
                        ViewPrivateAnswers,
                        QuestionToSpam,
                        QuestionFromSpam,
                        ViewSpamQuestions,
                        AnswerToSpam,
                        AnswerFromSpam,
                        ViewSpamAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportQuestions,
                        ReportAnswers
                    }
                }
            };

        }

    }

}
