﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Stars.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Stars.Repositories
{

    class StarRepository : IStarRepository<Star>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<StarRepository> _logger;

        public StarRepository(
            IDbContext dbContext,
            ILogger<StarRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<Star> InsertUpdateAsync(Star star)
        {
            if (star == null)
            {
                throw new ArgumentNullException(nameof(star));
            }
                
            var id = await InsertUpdateInternal(
                star.Id,
                star.Name,
                star.ThingId,
                star.CancellationToken,
                star.CreatedUserId,
                star.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Star> SelectByIdAsync(int id)
        {
            Star star = null;
            using (var context = _dbContext)
            {
                star = await context.ExecuteReaderAsync<Star>(
                    CommandType.StoredProcedure,
                    "SelectStarById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            star = new Star();
                            star.PopulateModel(reader);
                        }

                        return star;
                    },
                    id);
                
            }

            return star;
        }

        public async Task<IPagedResults<Star>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<Star> results = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            $"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                results = await context.ExecuteReaderAsync<IPagedResults<Star>>(
                    CommandType.StoredProcedure,
                    "SelectStarsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<Star>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Star();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                            return output;
                        }

                        return null;
                    },
                    inputParams);


            }

            return results;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting star with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteStarById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<Star>> SelectByNameAndThingId(string name, int thingId)
        {
            IList<Star> results = null;
            using (var context = _dbContext)
            {
                results = await context.ExecuteReaderAsync<List<Star>>(
                    CommandType.StoredProcedure,
                    "SelectStarsByNameAndThingId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new List<Star>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Star();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                            return output;
                        }

                        return null;

                    },
                    name,
                    thingId);

            }

            return results;
        }

        public async Task<Star> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            Star star = null;
            using (var context = _dbContext)
            {
                star = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectStarByNameThingIdAndCreatedUserId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            star = new Star();
                            star.PopulateModel(reader);
                        }

                        return star;
                    },
                    name,
                    thingId,
                    createdUserId);
                
            }

            return star;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string name,
            int thingId,
            string cancellationToken,
            int createdUserId,
            DateTimeOffset createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateStar",
                    id,
                    name.ToEmptyIfNull().TrimToSize(255),
                    thingId,
                    cancellationToken.ToEmptyIfNull().TrimToSize(100),
                    createdUserId,
                    createdDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion
        
    }
}
