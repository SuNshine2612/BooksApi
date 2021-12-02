using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BooksApi.Models.Book
{
    [BsonIgnoreExtraElements]
    [BsonCollection("Book")]
    public class Book
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Mã Sách")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [MaxLength(10, ErrorMessage = "{0} Tối Đa {1} Kí Tự")]
        [MinLength(3, ErrorMessage = "{0} Tối Thiểu {1} Kí Tự")]
        public string Code { get; set; }

        [Display(Name = "Tên Sách")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        public string Name { get; set; }

        [Display(Name = "Giá")]
        [BsonRepresentation(BsonType.Decimal128)]
        [Range(0, double.PositiveInfinity, ErrorMessage = "{0} phải lớn hơn {1}.")]
        public decimal Price { get; set; }

        [Display(Name = "Thể Loại")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Category { get; set; }

        // Không lưu vào data
        [BsonExtraElements]
        [BsonIgnore]
        [Display(Name = "Thể Loại")]
        public string CategoryName { get; set; }

        // Không lưu vào data
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public List<Category> ObjCategories { get; set; }

        [Display(Name = "Tác Giả")]
        [Required(ErrorMessage = "{0} Không Được Để Trống !")]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Author { get; set; }

        // Không lưu vào data
        [BsonExtraElements]
        [BsonIgnore]
        [Display(Name = "Tác Giả")]
        public string AuthorName{ get; set; }

        // Không lưu vào data
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public List<UserTest> ObjAuthors { get; set; } = null;

        [Display(Name = "Hình Ảnh")]
        public string UrlImage { get; set; }

        /// <summary>
        /// Nội dung HTML 
        /// </summary>
        [Display(Name = "Nội Dung")]
        public string Content { get; set; }

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
