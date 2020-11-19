using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.TMS.BASIC.DANHMUC
{
    [BsonIgnoreExtraElements]
    public class Distance
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        /// <summary>
        /// Mã tuyến đường
        /// </summary>
        [Display(Name = "Mã Tuyến Đường")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        /// <summary>
        /// Tên tuyến đường
        /// </summary>
        [Display(Name = "Tên Tuyến Đường")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// Xuất phát từ đâu
        /// </summary>
        [Display(Name = "Địa Điểm Đi")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string LocationCodeFrom { get; set; }

        /// <summary>
        /// Xuất phát đến đâu
        /// </summary>
        [Display(Name = "Địa Điểm Đến")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string LocationCodeTo { get; set; }

        [BsonExtraElements]
        [BsonIgnoreIfNull]
        [BsonIgnore]
        [Display(Name = "Địa Điểm Đi")]
        public string StrLocationFrom { get; set; }

        [BsonExtraElements]
        [BsonIgnoreIfNull]
        [BsonIgnore]
        [Display(Name = "Địa Điểm Đến")]
        public string StrLocationTo { get; set; }

        [Display(Name = "Khoảng Cách")]
        [BsonRepresentation(BsonType.Double)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public double DistanceKm { get; set; }

        [Display(Name = "Hao Hụt")]
        [BsonRepresentation(BsonType.Decimal128)]
        public double Loss { get; set; }

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
