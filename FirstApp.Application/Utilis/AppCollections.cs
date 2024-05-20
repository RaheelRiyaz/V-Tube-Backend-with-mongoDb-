using FirstApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Utilis
{
    public class AppCollections
    {
        public static Dictionary<Type, string> CollectionNames = new Dictionary<Type, string>
        {
        { typeof(User), "Users" },
        { typeof(Video), "Videos" },
        { typeof(Channel), "Channels" },
        { typeof(PlayList), "PlayLists" },
        { typeof(Views), "Views" },
        { typeof(Comment), "Comments" },
        { typeof(WatchHistory), "WatchHistory"},
        { typeof(CommentReplies), "CommentReplies"},
        { typeof(Likes), "Likes"},
        { typeof(Subscriber), "Subscribers"},
        { typeof(Notification), "Notifications"},
        { typeof(WatchLater), "WatchLater"},
        { typeof(Report), "Reports"},
    };
    }


}
