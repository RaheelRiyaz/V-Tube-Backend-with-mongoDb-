
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IViewsService
    {
        Task<APIResponse<int>> AddView(ViewRequest model);
        Task<APIResponse<List<ViewAPIResponse>>> FetchVideoViews(CommentsFilter model);
    }
}
