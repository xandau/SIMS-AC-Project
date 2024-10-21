using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI.AuthServices;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize]
    public class UserController : AController<User>
    {
        private JwtService jwtService;

        public UserController(IRepository<User> repository) : base(repository)
        {
            jwtService = new JwtService();
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<User>> ReadAsync([Required] int id)
        {
            string access_token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();

            long token_id = jwtService.GetClaimsFromToken(access_token);

            User? access_user = await _repository.GetAsync(token_id);

            if (access_user.Role != Enums.ERoles.ADMIN && token_id != id)
                return Forbid();
                

            User? e = await _repository.GetAsync(id, access_token);

            if (e == null)
                return UnprocessableEntity();
            else
                return Ok(e);
        }

        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete([Required] int id)
        {
            User ? e = await _repository.GetAsync(id);

            if (e is null)
                return UnprocessableEntity();

            if (e.Blocked == false)
                e.Blocked = true;
            
            await _repository.UpdateAsync(e);
            return NoContent();
        }
    }
}
