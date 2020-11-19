using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.BASIC.TMS.KHO_VATTU
{

    public class SupplierMaterial
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Mã NCC")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        [Display(Name = "Tên NCC")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// Số Điện Thoại
        /// </summary>
        [Display(Name = "Điện Thoại")]
        [BsonDefaultValue("")]
        public string Phone { get; set; }

        /// <summary>
        /// Địa Chỉ
        /// </summary>
        [Display(Name = "Địa Chỉ")]
        [BsonDefaultValue("")]
        public string Address { get; set; }

        /// <summary>
        /// Thư Điện Tử
        /// </summary>
        [BsonDefaultValue("")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        /// <summary>
        /// Fax
        /// </summary>
        [BsonDefaultValue("")]
        public string Fax { get; set; }

        /// <summary>
        /// Người đại diện
        /// </summary>
        [Display(Name = "Người Đại Diện")]
        [BsonDefaultValue("")]
        public string Representative { get; set; }

        /// <summary>
        /// Mô tả chi tiết thông tin Nhà Cung Cấp
        /// </summary>
        [Display(Name = "Ghi Chú")]
        [BsonDefaultValue("")]
        public string Desc { get; set; }

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
