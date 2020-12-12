﻿using SelfHosting.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SelfHosting.Services
{
    public class CustomerJobService : ICustomerJobService
    {
        private readonly ICustomerJobRepository _customJobRepository;
        public CustomerJobService(ICustomerJobRepository customJobRepository)
        {
            _customJobRepository = customJobRepository;
        }
        public List<CustomerJob> GetAll() => _customJobRepository.GetAll();

        public async Task<dynamic> AssignJob(int customerId, int jobId, string cron = "")
        {
            return await _customJobRepository.AssignJob(customerId,jobId,cron);
        }
    }
}
