﻿using SelfHosting.Common.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfHosting.Repository
{
    public interface ICustomerJobRepository
    {
        List<CustomerJob> GetAll();
        Task<dynamic> AssignJob(AssignJobRequest assignJobRequest);
    }
}
