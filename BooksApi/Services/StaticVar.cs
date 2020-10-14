using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Services
{
    public static class StaticVar
    {
        public static string CookiesName = "cookies_web_tms";
        public static string CookiesAuthenticate = "cookies_auth";

        // Key session !!
        [Display(Description = "Lưu token khi login")]
        public static string SessionUserToken = "SuSu";
        public static string ExpiredToken = "ExpiredToken";

        // Define Url Api !!
        public static readonly string ApiUrlBooks = "api/books";
        public static readonly string ApiUrlUsers = "api/userstest";
        public static readonly string ApiUrlCustomers = "api/customerstest";
        public static readonly string ApiUrlMenus = "api/menustest";
        public static readonly string ApiUrlSystemFunctions = "api/systemfunctionstest";
        public static readonly string ApiUrlGroups = "api/groupstest";

        // Define string alert, action and button !!
        public static readonly string ActionCreateNew = "Thêm Mới";
        public static readonly string ActionUpdate = "Cập Nhật";
        public static readonly string ActionDelete = "Xóa";

        public static readonly string ButtonSave = "Lưu";
        public static readonly string ButtonClose = "Đóng";

        public static readonly string MessageOk = "Đã Lưu Thành Công";
        public static readonly string MessageNotFound = "Không Tìm Thấy Đối Tượng";
        public static readonly string MessageCodeDuplicated = "Code Bị Trùng";

        // Define Key Claims !!
        public static readonly string ClaimObjectId = "ObjectId";
        public static readonly string ClaimCode = "Id";
        public static readonly string ClaimName = "FullName";
        public static readonly string ClaimEmail = "Email";
    }
}
