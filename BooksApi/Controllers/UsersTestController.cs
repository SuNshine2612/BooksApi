using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Test;
using BooksApi.Models.TMS;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersTestController : ControllerBase
    {
        private readonly GenericService<UserTest> _serviceUserTest;

        public UsersTestController()
        {
            _serviceUserTest = new GenericService<UserTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<UserTest>> Get()
        {
            return await _serviceUserTest.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<UserTest> GetDetails(string id)
        {
            UserTest data = await _serviceUserTest.GetByIDAsync(id);
            return data;
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
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(StaticVar.ClaimObjectId, user.Id));
            claims.Add(new Claim(StaticVar.ClaimCode, user.Code));
            claims.Add(new Claim(StaticVar.ClaimEmail, user.Email));
            claims.Add(new Claim(StaticVar.ClaimName, user.FullName));

            return claims;
 
        }
        #endregion
    }
}