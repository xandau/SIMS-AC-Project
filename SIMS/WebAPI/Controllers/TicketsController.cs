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
        private readonly ITicketRepository _ticketRepository;

        public TicketsController(ITicketRepository repository) : base(repository)
        {
            _ticketRepository = repository;
        }

        [HttpGet("assigned")]
        public async Task<ActionResult<List<Ticket>>> GetAssignedTicketByUser()
        {
            string? accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access Token not found");
            }
            else
            {
                try
                {
                    return Ok(await _ticketRepository.GetAssignedTickets(accessToken));
                }
                catch (Exception ex)
                {

                    return UnprocessableEntity(ex.Message);
                }
            }
        }

        [HttpGet("created")]
        public async Task<ActionResult<List<Ticket>>> GetCreatedTicketByUser()
        {
            string? accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access Token not found");
            }
            else
            {
                try
                {
                    return Ok(await _ticketRepository.GetCreatedTickets(accessToken));
                }
                catch (Exception ex)
                {

                    return UnprocessableEntity(ex.Message);
                }
            }
                
        }
    }
}
