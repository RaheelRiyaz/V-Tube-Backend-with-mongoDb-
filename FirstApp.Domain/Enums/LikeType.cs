using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Enums
{
    public enum LikeType : byte
    {
        Video = 1,
        Comment = 2,
    }

    public enum CommentType : byte 
    {
        Comment = 1,
        Reply = 2,
    }

    public enum ReportType : byte
    {
        Video = 1,
        Comment = 2,
        Reply = 3,
    }
}
