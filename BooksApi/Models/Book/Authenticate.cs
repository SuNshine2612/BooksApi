using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Book
{
    public class Authenticate
    {
        /// <summary>
        /// Username / phone
        /// </summary>
        [Display(Name = "Tên Đăng Nhập")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        public string Code { get; set; }

        /// <summary>
        /// Mật khẩu cũ, có thể null
        /// </summary>
        [Display(Name = "Mật Khẩu Cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
