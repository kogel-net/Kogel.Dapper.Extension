using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension;
using Newtonsoft.Json;
using zgh_service_api.Models;

namespace zgh_service_api.DTO
{
    public class UserDetailDTO: IBaseEntity<User, int>
    {
        //public int Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        [JsonIgnore]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public UserDetailDTO()
        {
        }

        public UserDetailDTO(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            FullName = user.FullName;
            RoleId = user.RoleId;
        }
    }
}
