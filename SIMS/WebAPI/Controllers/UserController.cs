using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
