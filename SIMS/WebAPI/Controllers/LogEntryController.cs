using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("log")]
    [Authorize]
    public class LogEntryController : AController<LogEntry>
    {
        public LogEntryController(IRepository<LogEntry> repository) : base(repository)
        {

        }
    }
}
