﻿using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.ViewModels;

namespace Plato.Search.Services
{
    public interface ISearchService
    {

        Task<IPagedResults<Entity>> GetResultsAsync(SearchIndexOptions options, PagerOptions pager);

    }
    
}
