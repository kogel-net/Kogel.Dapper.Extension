using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test
{
   public class Comment
    {
        [Identity]
        public int Id { get; set; }
        /// <summary>
        /// 评论父级id，不为0则是回复评论
        /// </summary>
        public int PId { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; }
        public DateTime SubTime { get; set; } = DateTime.Now;
        public int Type { get; set; }
        /// <summary>
        /// 咨询id
        /// </summary>
        public int ArticleId { get; set; }
        public int StarCount { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public int ArticleUserId { get; set; }
        public int ReplayCount { get; set; }
        public int LikeCount { get; set; }
        public bool IsRobot { get; set; }
        public string IdentityId { get; set; }
        public bool CurrentUserLikes { get; set; }
        /// <summary>
        /// 关联的评论id
        /// </summary>
        public long RefCommentId { get; set; }
    }
}
