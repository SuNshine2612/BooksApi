using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Book
{
    public class CustomerTest
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Mã")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Code { get; set; }

        [Display(Name = "Họ Tên")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Email { get; set; }

        [Display(Name = "Điện Thoại")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Phone { get; set; }

        [Display(Name = "Công Ty")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Company { get; set; }

        [Display(Name = "Hình")]
        public string UrlImage { get; set; }

        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        //==============================================
        /// <summary>
        /// Trạng thái, mặc định = 0
        /// </summary>
        [BsonRepresentation(BsonType.Int32)]
        public int Status { get; set; } = 0;
        /// <summary>
        /// Đang Active không? mặc định = true là đang active
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Tạo bởi ai
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Sửa / Xóa bởi ai
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// thời điểm sửa/xóa
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdatedOn { get; set; }
    }
}