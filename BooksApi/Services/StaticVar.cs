using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Services
{
    public static class StaticVar
    {
        public const string CookiesName = "cookies_web_books";
        public const string CookiesAuthenticate = "cookies_auth";
        public static readonly string NameMenuActive = "Scheduler";

        // Key session !!
        [Display(Description = "Lưu token khi login")]
        public const string SessionUserToken = "SuSu";
        public const string ExpiredToken = "ExpiredToken";

        // Define Url Api !!
        public static readonly string ApiUrlBooks = "api/books";
        public static readonly string ApiUrlUsers = "api/userstest";
        public static readonly string ApiUrlCustomers = "api/customerstest";
        public static readonly string ApiUrlMenus = "api/menustest";
        public static readonly string ApiUrlSystemFunctions = "api/systemfunctionstest";
        public static readonly string ApiUrlGroups = "api/groupstest";
        public static readonly string ApiUrlCategories = "api/categories";
        public static readonly string ApiUrlSlides = "api/slides";
        public static readonly string ApiUrlComments = "api/comments";
        public static readonly string ApiUrlNews = "api/news";
        public static readonly string ApiUrlConfigs = "api/configs";

        // Define string alert, action and button !!
        public static readonly string ActionCreateNew = "Thêm Mới";
        public static readonly string ActionUpdate = "Cập Nhật";
        public static readonly string ActionDelete = "Xóa";
        public static readonly string ActionYes = "Có";
        public static readonly string ActionNo = "Không";

        public static readonly string ButtonSave = "Lưu";
        public static readonly string ButtonClose = "Đóng";

        public static readonly string MessageOk = "Đã Lưu Thành Công";
        public static readonly string MessageNotFound = "Không Tìm Thấy Đối Tượng";
        public static readonly string MessageCodeDuplicated = "Code Bị Trùng";
        public static readonly string MessageOkLoadData = "Tải Thành Công";

        // Define Key Claims !!
        public static readonly string ClaimObjectId = "ObjectId";
        public static readonly string ClaimCode = "Id";
        public static readonly string ClaimName = "FullName";
        public static readonly string ClaimEmail = "Email";
        public static readonly string ClaimArrFunction = "ArrFunction";
        public static readonly string ClaimArrMenu = "ArrMenu";
        public static readonly string ClaimArrMemberGroup = "ArrMemberGroup";


        public const string MainSlide = "Main Slide";
        public const string SubSlide = "Sub Slide";
    }
}
