﻿using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IReportsService
    {
        Task<APIResponse<int>> AddReport(ReportRequest model);
    }
}