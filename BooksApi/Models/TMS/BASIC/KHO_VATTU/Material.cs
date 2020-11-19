using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.TMS.BASIC.KHO_VATTU
{
    [BsonIgnoreExtraElements]
    public class Material
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Mã Vật Tư")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        [Display(Name = "Tên Vật Tư")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// Mã Loại Vật Tư (Code)
        /// </summary>
        [Display(Name = "Loại Vật Tư")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string MaterialTypeId { get; set; }

        /// <summary>
        /// Tên loại vật tư - chỉ dùng để hiển thị, không có dưới database
        /// </summary>
        [Display(Name = "Loại Vật Tư")]
        [BsonExtraElements]
        [BsonIgnore]
        [BsonIgnoreIfNull]
        public string StrMaterialType { get; set; }

        /// <summary>
        /// Đơn vị tính Id (code)
        /// </summary>
        [Display(Name = "Đơn Vị Tính")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string UnitId { get; set; }

        /// <summary>
        /// Tên đơn vị tính - chỉ dùng để show lên giao diện, không có trong database
        /// </summary>
        [Display(Name = "Đơn Vị Tính")]
        [BsonExtraElements]
        [BsonIgnore]
        [BsonIgnoreIfNull]
        public string StrUnitName { get; set; }

        /// <summary>
        /// Thông số khác
        /// </summary>
        [Display(Name = "Thông Số Khác")]
        [BsonDefaultValue("")]
        public string OtherInfo { get; set; }

        /// <summary>
        /// Mã Kho (Code)
        /// </summary>
        [Display(Name = "Kho Hàng")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string WarehouseId { get; set; }

        /// <summary>
        /// Tên kho - chỉ dùng để hiển thị, không có dưới database
        /// </summary>
        [Display(Name = "Kho Hàng")]
        [BsonExtraElements]
        [BsonIgnore]
        [BsonIgnoreIfNull]
        public string StrWarehouseName { get; set; }

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
