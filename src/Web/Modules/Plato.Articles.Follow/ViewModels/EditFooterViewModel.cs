﻿using PlatoCore.Security.Abstractions;

namespace Plato.Articles.Follow.ViewModels
{
    public class EditFooterViewModel
    {

        public bool IsNewEntity { get; set; }

        public string NotifyHtmlName { get; set; }

        public IPermission Permission { get; set; }
        
    }

}
