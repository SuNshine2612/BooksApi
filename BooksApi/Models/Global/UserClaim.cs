using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Global
{
    /// <summary>
    /// UserClaim: biến dùng chung cho cả project
    /// </summary>
    public static class UserClaim
    {
        /// <summary>
        /// Object Id từ mongo
        /// </summary>
        public static string ObjectId { get; set; }
        /// <summary>
        /// Số phone
        /// </summary>
        public static string UserId { get; set; }
        /// <summary>
        /// User token string
        /// </summary>
        public static JwtSecurityToken Token { get; set; }
        /// <summary>
        /// Mã công ty
        /// </summary>
        public static string CompanyId { get; set; }
        /// <summary>
        /// mã phòng ban
        /// </summary>
        public static string DepartmentId { get; set; }
        /// <summary>
        /// Token của thiết bị (firebase)
        /// </summary>
        public static string DeviceId { get; set; }
        /// <summary>
        /// Fullname
        /// </summary>
        public static string FullName { get; set; }

        /// <summary>
        /// reset all setting to null
        /// </summary>
        public static void Reset()
        {
            ObjectId = string.Empty;
            UserId = string.Empty;
            Token = null;
            CompanyId = string.Empty;
            DepartmentId = string.Empty;
            DeviceId = string.Empty;
            FullName = string.Empty;
        }
    }
}
