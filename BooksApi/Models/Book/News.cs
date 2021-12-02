using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Book
{
    public class News
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }

        [Display(Name = "Tiêu Đề")]
        public string Title { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string UrlImage { get; set; }

        [Display(Name = "Tóm Tắt")]
        public string Summary { get; set; }

        [Display(Name = "Nội Dung")]
        public string Content { get; set; }

        [BsonIgnoreIfDefault]
        [BsonIgnoreIfNull]
        public List<UserTest> ObjUsers { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Status { get; set; } = 0;
        /// <summary>
        /// Đang Active không? mặc định = true là đang active
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Tạo bởi ai
        /// </summary>
        [Display(Name = "Người Viết")]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Thời điểm tạo
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Display(Name = "Ngày Viết")]
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
