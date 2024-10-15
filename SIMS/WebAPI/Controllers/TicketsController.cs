using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("ticket")]
    [Authorize]
    public class TicketsController : AController<Ticket>
    {
        public TicketsController(IRepository<Ticket> repository) : base(repository)
        {

        }
    }
}
