using System;
using Dapper;
using Kogel.Dapper.Extension;
//using Dapper.Contrib.Extensions;
using Kogel.Dapper.Extension.Attributes;
using Newtonsoft.Json;

namespace zgh_service_api.Models
{
    [Display(Rename = "Role")]
    public class Role: IBaseEntity<Role, int>
    {
        //[Identity]
        //public int Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }
        [JsonIgnore]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
