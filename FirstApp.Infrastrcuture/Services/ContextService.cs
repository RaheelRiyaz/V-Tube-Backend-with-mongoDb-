using FirstApp.Application.Abstractions.IServices;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Infrastrcuture.Services
{
    public class ContextService (IHttpContextAccessor httpContextAccessor) : IContextService
    {
        public ObjectId GetUserId()
        {
            var userId = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(_ => _.Type == "Id")?.Value!;

            return userId is not null ? ObjectId.Parse(userId) : ObjectId.Empty;
        }
    }
}
