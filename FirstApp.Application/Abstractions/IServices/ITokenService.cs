using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface ITokenService
    {
        string GenerateAccessToken(User model);
        string GenerateRefreshToken(ObjectId id);
    }
}
