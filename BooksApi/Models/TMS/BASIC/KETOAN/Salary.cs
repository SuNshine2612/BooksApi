using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.TMS.BASIC.KETOAN
{
    public class Salary
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Mã Quản Lý")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        [Display(Name = "Tên Quản Lý")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        [Display(Name = "Bộ Phận")]
        [BsonDefaultValue("")]
        public string Department { get; set; }

        /// <summary>
        /// Ngày hiệu lực
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Lương")]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public decimal SalarySum { get; set; }

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
