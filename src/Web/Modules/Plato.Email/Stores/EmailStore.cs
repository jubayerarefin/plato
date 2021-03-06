﻿using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Email.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Emails.Abstractions;

namespace Plato.Email.Stores
{

    public class EmailStore : IEmailStore<EmailMessage>
    {

        private readonly IEmailRepository<EmailMessage> _emailRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EmailStore> _logger;
        
        public EmailStore(
            IEmailRepository<EmailMessage> emailRepository,
            ICacheManager cacheManager,
            ILogger<EmailStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _emailRepository = emailRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EmailMessage> CreateAsync(EmailMessage model)
        {
            var result = await _emailRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<EmailMessage> UpdateAsync(EmailMessage model)
        {
            var result = await _emailRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(EmailMessage model)
        {

            var success = await _emailRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted email '{0}' with id {1}",
                        model.Subject, model.Id);
                }

                CancelTokens(model);
            }

            return success;

        }

        public async Task<EmailMessage> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _emailRepository.SelectByIdAsync(id));
        }

        public IQuery<EmailMessage> QueryAsync()
        {
            var query = new EmailQuery(this);
            return _dbQuery.ConfigureQuery<EmailMessage>(query); ;
        }

        public async Task<IPagedResults<EmailMessage>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _emailRepository.SelectAsync(dbParams));
        }

        public void CancelTokens(EmailMessage model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }
}
