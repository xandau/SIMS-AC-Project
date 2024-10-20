using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize]
    public class UserController : AController<User>
    {
        public UserController(IRepository<User> repository) : base(repository)
        {

        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<User>> ReadAsync([Required] int id)
        {
            string access_token = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
            
            try
            {
                User? e = await _repository.GetAsync(id, access_token);

                if (e == null)
                    return UnprocessableEntity();
                else
                    return Ok(e);
            }
            catch (Exception ex)
            {
                return Forbid();
            }
        }
    }
}
