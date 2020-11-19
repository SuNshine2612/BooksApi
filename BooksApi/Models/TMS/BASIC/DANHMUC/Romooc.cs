using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace BooksApi.Models.TMS.BASIC.DANHMUC
{
    public class Romooc
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Mã Số Romooc")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        [Display(Name = "Số Romooc")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        [Display(Name = "Nhãn Hiệu")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Label { get; set; }

        [Display(Name = "Loại Hình Romooc")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string CategoryRomooc { get; set; }

        [Display(Name = "Chủng Loại")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Type { get; set; }

        [Display(Name = "Nơi Sản Xuất")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string MadeIn { get; set; }

        [Display(Name = "Tải Trọng")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public decimal Loads { get; set; }

        [Display(Name = "Trọng Lượng")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public decimal Weight { get; set; }

        [Display(Name = "Số Khung")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string FrameNumber { get; set; }

        /// <summary>
        /// Ngày kiểm định
        /// </summary>
        [Display(Name = "Ngày Kiểm Định")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [DisplayFormat(DataFormatString = "dd/MMM/yyyy")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime InspectionDate { get; set; }

        /// <summary>
        /// Số kiểm định
        /// </summary>
        [Display(Name = "Số Kiểm Định")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string InspectionNumber { get; set; }

        /// <summary>
        /// Thời hạn kiểm định
        /// </summary>
        [Display(Name = "Thời Hạn Kiểm Định")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [DisplayFormat(DataFormatString = "dd/MMM/yyyy")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime InspectionDeadline { get; set; }

        /// <summary>
        /// Chi phí kiểm định
        /// </summary>
        [Display(Name = "Chi Phí Kiểm Định")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public decimal InspectionCost { get; set; }

        /// <summary>
        /// Thông số khác
        /// </summary>
        [Display(Name = "Thông Số Khác")]
        [BsonDefaultValue("")]
        public string OtherInfo { get; set; }

        /// <summary>
        /// Địa điểm hiện tại (Code)
        /// </summary>
        [Display(Name = "Địa Điểm Hiện Tại")]
        [BsonDefaultValue("")]
        public string LocationCurrentCode { get; set; }

        /// <summary>
        /// Tên Địa điểm hiện tại - chỉ dùng để hiển thị, không có dưới database
        /// </summary>
        [Display(Name = "Địa Điểm Hiện Tại")]
        [BsonExtraElements]
        [BsonIgnore]
        [BsonIgnoreIfNull]
        public string StrLocationCurrentCode { get; set; }

        /// <summary>
        /// Địa điểm trước (Code)
        /// </summary>
        [Display(Name = "Địa Điểm Trước")]
        [BsonDefaultValue("")]
        public string LocationBeforeCode { get; set; }

        /// <summary>
        /// Tên Địa điểm trước - chỉ dùng để hiển thị, không có dưới database
        /// </summary>
        [Display(Name = "Địa Điểm Trước")]
        [BsonExtraElements]
        [BsonIgnore]
        [BsonIgnoreIfNull]
        public string StrLocationBeforeCode { get; set; }

        /// <summary>
        /// Số Cont
        /// </summary>
        [Display(Name = "Số Cont")]
        [BsonDefaultValue("")]
        [MaxLength(11, ErrorMessage = "{0} Tối Đa {1} Kí Tự")]
        [MinLength(11, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string ContNumber { get; set; }

        /// <summary>
        /// Chi phí mua
        /// </summary>
        [Display(Name = "Chi Phí Mua")]
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonDefaultValue(0)]
        public decimal Price { get; set; }

        /// <summary>
        /// Ngày sử dụng
        /// </summary>
        [Display(Name = "Ngày Sử Dụng")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime HolidayDate { get; set; }

        /// <summary>
        /// Số Tháng Khấu Hao
        /// </summary>
        [Display(Name = "Thứ Tự")]
        [BsonRepresentation(BsonType.Int32)]
        public int Depreciation { get; set; }

        /// <summary>
        /// Phí bh hàng tháng
        /// </summary>
        [Display(Name = "Phí BH Mooc Tháng")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal WarrantyExpenses { get; set; }

        /// <summary>
        /// Ghi Chú
        /// </summary>
        [Display(Name = "Ghi Chú")]
        [BsonDefaultValue("")]
        public string Note { get; set; }

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
