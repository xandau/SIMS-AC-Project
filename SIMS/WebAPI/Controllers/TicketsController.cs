using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI.DTOs;
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

        [HttpPost("stop")]
        public async Task<ActionResult> StopInstance([Required] StopInstance request)
        {
            string? accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().ToString();
            try
            {
                await _ticketRepository.StopInstance(accessToken, request);
                return Ok("Instance deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Something went wrong calling Lambda.");
            }
            
        }
    }
}
