using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Book
{
    //[BsonCollection("Config")]
    public class Config
    {
        [BsonId] // designate this property as the document's primary key.
        [BsonRepresentation(BsonType.ObjectId)] // allow passing the parameter as type string instead of an ObjectId structure
        public string Id { get; set; }
       
        [Display(Name = "Mã")]
        public string Code { get; set; }

        [Display(Name = "Tên Công Ty / Cửa Hàng")]
        public string CompanyName { get; set; }

        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Display(Name = "Logo")]
        public string UrlImage { get; set; }

        [Display(Name = "Điện Thoại")]
        public string Phone { get; set; }

        public string Hotline { get; set; }
        public string Fax { get; set; }
        public string Fanpage { get; set; }
        public string Email { get; set; }

        [Display(Name = "Chèn Code Javascripts")]
        public string ScriptCode { get; set; }

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
