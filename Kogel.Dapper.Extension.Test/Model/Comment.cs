using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension.Test.Model
{

    public class BaseEntity<T> : IBaseEntity<T, int>
    {
        [Identity]
        public override int Id { get; set; }
    }


    public class Comment : BaseEntity<Comment>
    {
        [Identity(IsIncrease = true)]
        public override int Id { get => base.Id; set => base.Id = value; }
        /// <summary>
        /// 评论父级id，不为0则是回复评论
        /// </summary>
        public int? PId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        [Display(Rename = "SubTime")]
        public DateTime? SubTime { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 咨询id
        /// </summary>
        public int ArticleId { get; set; }
        public int StarCount { get; set; } = 0;
        public bool? IsDeleted { get; set; } = false;
        public int ArticleUserId { get; set; }

        [Display(Rename = "ReplayCount")]
        public int ReplayCount111 { get; set; }
        public int LikeCount { get; set; }
        public bool IsRobot { get; set; }
        [Display(IsNull = true)]
        public string IdentityId { get; set; }

        [Display(IsField = false)]
        public bool CurrentUserLikes { get; set; }
        /// <summary>
        /// 关联的评论id
        /// </summary>
        public int RefCommentId { get; set; }

        [ForeignKey("ArticleId", "Id")]
        public virtual News News { get; set; }

        [Display(IsField = false)]
        public Guid Guid { get; set; }
    }




    [Display(Rename = "Comment", AsName = "commm5")]
    public class Comment1 : Comment
    {

    }

    [Display(Rename = "Comment", AsName = "commm5")]
    public class Comment2 : Comment
    {
        /// <summary>
        /// 新闻类
        /// </summary>
        [ForeignKey("ArticleId", "Id")]
        public override News News { get; set; }

        /// <summary>
        /// 资源类
        /// </summary>
        [ForeignKey("Id", "FKId")]
        public List<ResourceMapping> ResourceMapping { get; set; }
    }

}
