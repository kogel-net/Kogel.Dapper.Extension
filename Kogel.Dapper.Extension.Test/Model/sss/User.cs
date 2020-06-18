using System;
using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;

namespace zgh_service_api.Models
{
    [Display(Rename = "User")]
    public class User: IBaseEntity<User, int>
    {
        //[Identity]
        //public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public int RoleId { get; set; }

        //[ForeignKey("RoleId", "Id")] // 当前版本有Bug
        //public Role Role { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
