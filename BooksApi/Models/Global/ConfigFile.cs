using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Global
{
    public static class ConfigFile
    {
        /// <summary>
        /// Khóa bí mật
        /// </summary>
        public static string SecretKey { get; set; }
        /// <summary>
        /// Nhà phát hành
        /// </summary>
        public static string Issuer { get; set; }
        /// <summary>
        /// Audience
        /// </summary>
        public static string Audience { get; set; }
        /// <summary>
        /// Token path
        /// </summary>
        public static string TokenPath { get; set; }
        /// <summary>
        /// Cookie name
        /// </summary>
        public static string CookieName { get; set; }
        /// <summary>
        /// Chuỗi kết nối DB
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// Tên DB
        /// </summary>
        public static string Database { get; set; }
        /// <summary>
        /// MongoClient
        /// </summary>
        public static MongoClient MyMongoClient { get; set; }
        /// <summary>
        /// MongoDatabase
        /// </summary>
        public static IMongoDatabase MyMongoDatabase { get; set; }
    }
}
