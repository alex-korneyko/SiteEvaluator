using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SiteEvaluator.Data.Model;

namespace SiteEvaluator.Data.DataHandlers
{
    public class DbDataHandlerService : IDataHandlerService
    {
        private readonly IRepository<TargetHost> _repository;

        public DbDataHandlerService(IRepository<TargetHost> repository)
        {
            _repository = repository;
        }
        
        public IEnumerable<TargetHost> GetAllAsync()
        {
            return _repository.GetAll();
        }
        
        public async Task<TargetHost> GetTargetHostAsync(Uri hostUri)
        {
            var targetHost = await _repository.GetAll().FirstOrDefaultAsync(host => host.HostUrl.Equals(hostUri.Host));

            return targetHost;
        }
        
        public async Task<TargetHost> SaveTargetHostAsync(TargetHost targetHost)
        {
            var targetHostUriForSave = new Uri(targetHost.HostUrl);
            var targetHostFromDb = await GetTargetHostAsync(targetHostUriForSave);

            if (targetHostFromDb != null)
            {
                _repository.Delete(targetHostFromDb);
            }

            targetHost.PageInfos.ForEach(pageInfo => pageInfo.ClearContent());
            
            await _repository.AddAsync(targetHost);

            return await GetTargetHostAsync(targetHostUriForSave);
        }
    }
}