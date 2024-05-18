using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IRepositories
{
    public interface ICommentAbuserepository
    {
        Task<bool> IsAbusiveComment(string comment);
    }
}
