using BooksApi.Models.Global;
using BooksApi.Models.Book;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersTestController : ControllerBase
    {
        private readonly GenericService<UserTest> _serviceUserTest;
        private readonly GenericService<GroupTest> _serviceGroupTest;

        public UsersTestController()
        {
            _serviceUserTest = new GenericService<UserTest>();
            _serviceGroupTest = new GenericService<GroupTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<UserTest>> Get()
        {
            return await _serviceUserTest.GetAllAsync();
        }

        /// <summary>
        /// Lấy 1  thành viên dựa vào Id
        /// </summary>
        /// <param name="id">Object Id dạng string</param>
        /// <returns></returns>
        // GET: api/Users/id
        [HttpGet("{id}")]
        public async Task<UserTest> Get(string id)
        {
            UserTest user = await _serviceUserTest.GetByIDAsync(id).ConfigureAwait(false);
            if (user != null)
            {
                user.Password = String.Empty;
            }
            return user;
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<UserTest> GetDetails(string id)
        {
            UserTest data = await _serviceUserTest.GetByIDAsync(id);
            return data;
        }

        /// <summary>
        /// Get list user by group Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>

        [HttpGet("GetListUserByGroup/{groupId}")]
        public async Task<List<UserTest>> GetListUserByGroup(string groupId)
        {
            return await _serviceUserTest._collection.Find(user => user.ArrMemberGroup.Contains(groupId)).ToListAsync();
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<UserTest>> Post([FromBody] UserTest data)
        {

            try
            {
                data.Code = data.Code.ToLower().Trim();
                if (!String.IsNullOrEmpty(data.Email))
                {
                    data.Email = data.Email.ToLower()?.Trim();
                }
                data.Password = CustomPasswordHasher.HashPassword(data.Password);
                data.CreatedOn = DateTime.Now;
                //data.CreatedBy = UserClaim.UserId;

                return await _serviceUserTest.InsertAsync(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(string id, [FromBody] UserTest value)
        {
            try
            {
                //kiểm tra giống ID hay không
                if (id.Equals(value.Id, StringComparison.OrdinalIgnoreCase))
                {
                    //get user from DB
                    UserTest dbUser = await _serviceUserTest.GetByIDAsync(value.Code);

                    value.Code = value.Code.ToLower().Trim();
                    if (!String.IsNullOrEmpty(value.Email))
                    {
                        value.Email = value.Email.ToLower()?.Trim();
                    }
                    //CỐ TÌNH - ÉP KHÔNG CHO ĐỔI PASSWORD Ở ĐÂY
                    value.Password = dbUser.Password;
                    //cập nhật UpdatedOn
                    value.UpdatedOn = DateTime.Now;
                    value.UpdatedBy = UserClaim.UserId;
                    //gọi hàm update
                    var result = await _serviceUserTest.UpdateAsync(id, value);
                    return Ok(result);
                }
                return BadRequest(StaticVar.MessageNotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="user">Object User</param>
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] Authenticate user)
        {
            if (user == null)
                return BadRequest(StaticVar.MessageNotFound);
            //get user from DB
            UserTest dbUser = await _serviceUserTest.GetByIDAsync(user.Code).ConfigureAwait(false);
            if (dbUser == null)
                return BadRequest(StaticVar.MessageNotFound);
            //kiểm tra password cũ có đúng không
            if (CustomPasswordHasher.VerifyPassword(dbUser.Password, user.OldPassword))
            {
                var passwordHashed = CustomPasswordHasher.HashPassword(user.Password);
                //đúng thì cho đổi password
                BsonDocument objBSON = new BsonDocument
                    {
                        { "Password",  passwordHashed }
                    };
                await _serviceUserTest.UpdateCustomizeFieldByIdAsync(dbUser.Id, objBSON);
                return Ok("Đổi password thành công");
            }
            else
            {
                //sai thì báo lỗi
                return BadRequest("User hoặc mật khẩu cũ không hợp lệ");
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var find = await _serviceUserTest.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceUserTest.DeleteAsync(id));
                }
                return BadRequest(StaticVar.MessageNotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Login Functions
        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<bool> ExistsCode(string id)
        {
            return await _serviceUserTest.CheckIssetByID(id);
        }


        // POST: api/UsersTest
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="authenticate"></param>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Authenticate authenticate)
        {
            //reset
            HttpContext.User = null;
            UserClaim.Reset();

            string userToken = await LoginUser(authenticate?.Code?.Trim().ToLower(CultureInfo.CurrentCulture), authenticate?.Password?.Trim()).ConfigureAwait(false);
            if (string.IsNullOrEmpty(userToken))
            {
                return BadRequest(StaticVar.MessageNotFound);
            }
            else
                return Ok(userToken);
        }

        [NonAction]
        private async Task<string> LoginUser(string userID, string clearPassword)
        {
            if (string.IsNullOrEmpty(clearPassword))
                clearPassword = string.Empty;

            UserTest user = await _serviceUserTest.GetByIDAsync(userID).ConfigureAwait(false);// lstUser.SingleOrDefault(x => x.Code == userID);
            //Xác thực User, nếu chưa đăng ký thì trả về false
            if (user == null || !CustomPasswordHasher.VerifyPassword(user.Password, clearPassword))
            {
                return string.Empty;
            }
            return GenerateToken(user);
        }

        [NonAction]
        private protected string GenerateToken(UserTest user)
        {
            //Đọc Private Key trong Startup.cs, 
            var key = ConfigFile.SecretKey;//_configuration.GetSection("TokenAuthentication:SecretKey").Value;

            var secretKey = Encoding.ASCII.GetBytes(key);

            //Generate Token for user 
            var JWToken = new JwtSecurityToken(
                issuer: ConfigFile.Issuer,//_configuration.GetSection("TokenAuthentication:Issuer").Value,
                audience: ConfigFile.Audience,// _configuration.GetSection("TokenAuthentication:Audience").Value,
                claims: GenerateUserClaims(user),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Today.AddDays(2)).DateTime,
                //Using HS256 Algorithm to encrypt Token
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;
        }

        [NonAction]
        private IEnumerable<Claim> GenerateUserClaims(UserTest user)
        {
            List<Claim> claims = new()
            {
                new Claim(StaticVar.ClaimObjectId, user.Id), // ObjectID
                new Claim(StaticVar.ClaimCode, user.Code), // Username
                new Claim(StaticVar.ClaimEmail, user.Email),
                new Claim(StaticVar.ClaimName, user.FullName)
            };

            //tìm memberGroup theo user
            if (user.ArrMemberGroup != null && user.ArrMemberGroup.Length > 0)
            {
                //1. Lấy danh sách member group mà user này thuộc về
                List<string> listGroupsBelongToUser = new List<string>(user.ArrMemberGroup);
                List<GroupTest> memberGroups = _serviceGroupTest.SearchMatchArray_NotAsync("Code", listGroupsBelongToUser);

                //2. Xử lý trùng code trong ArrFunctionId và MenuID của group, xong gán vào Claims
                string[] strArrFunction = Array.Empty<string>();
                string[] strArrMenu = Array.Empty<string>();

                foreach (var member in memberGroups)
                {
                    if (member.ArrFunctionId != null && member.ArrFunctionId.Length > 0)
                    {
                        //gom nhóm các chức năng lại, loại bỏ trùng (nếu muốn giữ trùng thì dùng Concat thay cho Union)
                        strArrFunction = strArrFunction.Union(member.ArrFunctionId).ToArray();
                        strArrMenu = strArrMenu.Union(member.ArrMenuId).ToArray();
                    }
                }

                claims.Add(new Claim(StaticVar.ClaimArrFunction, string.Join(",", strArrFunction)));
                claims.Add(new Claim(StaticVar.ClaimArrMenu, string.Join(",", strArrMenu)));
                // Gán thêm Groups của User trong trường hợp cần kiểm tra
                claims.Add(new Claim(StaticVar.ClaimArrMemberGroup, string.Join(",", user.ArrMemberGroup)));
            }

            return claims;

        }
        #endregion

        #region ROLE GROUP
        [HttpPost("SetGroup/{groupId}")]
        public async Task<IActionResult> SetGroup(string groupId, [FromBody] List<string> userSelected)
        {
            try
            {
                // Danh sách User tìm đc theo userSelected
                var listUsers = _serviceUserTest.SearchMatchArray_NotAsync("Code", userSelected);
                // Gán danh sách
                listUsers.ForEach(x =>
                {
                    x.ArrMemberGroup = (x.ArrMemberGroup == null || x.ArrMemberGroup.Length == 0) ? (new string[] { groupId }.ToArray()) : (x.ArrMemberGroup.Append(groupId).Distinct().ToArray());
                });
                // Cập nhật danh sách
                if (listUsers.Count > 0)
                {
                    await _serviceUserTest.UpdateManyAsync(listUsers).ConfigureAwait(false);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}