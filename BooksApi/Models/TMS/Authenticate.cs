using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BooksApi.Models.TMS
{
    /// <summary>
    /// Dùng cho login
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Authenticate
    {
        /// <summary>
        /// Username / phone
        /// </summary>
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [Display(Name = "Tên Đăng Nhập")]
        public string Code { get; set; }

        /// <summary>
        /// Mật khẩu cũ, có thể null
        /// </summary>
        [BsonDefaultValue(defaultValue: "")]
        [Display(Name = "Mật Khẩu Cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [Display(Name = "Mật Khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
