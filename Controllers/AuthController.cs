using cube_gaming_store_back.DTOs.Posts;
using cube_gaming_store_back.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cube_gaming_store_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuth auth) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> PostSignUpUser([FromBody] SignUpPostDTO newUser)
        {
            var findUser = await auth.FindUserByEmail(newUser.Email!);
            if (findUser)
            {
                return BadRequest(new { msg = "User already exist" });
            }
            var response = await auth.SignUp(newUser);
            if (!response.Flag)
            {
                return BadRequest(new { msg = response.Msg });
            }
            return Ok(new { msg = response.Msg });
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> PostSignInUser([FromBody] SignInPostDTO user)
        {
            var findUser = await auth.FindUserByEmail(user.Email!);
            if (!findUser)
            {
                return NotFound(new { msg = "User Not Found" });
            }
            var res = await auth.SignIn(user);
            if (res.Token == null)
            {
                return BadRequest(new { msg = res.Msg });
            }
            Response.Cookies.Append("token", res.Token, new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok(new { token = res.Token, msg = res.Msg });
        }
        [HttpPost("sign-out")]
        [Authorize]
        public async Task<IActionResult> PostSignOutUser()
        {
            var response = await auth.SignOut();
            if (!response)
            {
                return BadRequest(new { msg = "Something went wrong" });
            }
            Response.Cookies.Delete("token");
            return Ok(new { msg = "Log out successfully" });
        }
        [HttpPost("create-role")]
        [Authorize]
        public async Task<IActionResult> PostCreateRole([FromBody] string role)
        {
            var res = await auth.CreateRole(role);
            if (!res)
            {
                BadRequest(new { msg = "Something went wrong" });
            }
            return Ok(new { msg = "Role Create Successfully" });
        }
        // [HttpGet("get-users")]
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> GetUsers()
        // {
        //     var getUsers = await auth.ViewUsers();
        //     return Ok(new { data = getUsers });
        // }
        // [HttpGet("get-auth/{userId}")]
        // [Authorize]
        // public async Task<IActionResult> GetUser(string userId)
        // {
        //     var getUser = await auth.FindUserById(userId);
        //     if (!getUser) return NotFound(new { msg = "User Not Found" });
        //     var response = await auth.ViewUser(userId);
        //     return Ok(new { data = response });
        // }
        // [HttpPut("update-auth/{userId}")]
        // [Authorize]
        // public async Task<IActionResult> UpdateUser([FromRoute] string userId, [FromBody] UserUpdateDTO userUpdate)
        // {
        //     var findUser = await auth.FindUserById(userId);
        //     if (!findUser)
        //     {
        //         return NotFound(new { msg = "User Not Found" });
        //     }
        //     var response = await auth.EditUser(userId, userUpdate);
        //     if (!response)
        //     {
        //         return BadRequest(new { msg = "User update failed" });
        //     }
        //     return Ok(new { msg = "User update successfully" });
        // }
    }
}
