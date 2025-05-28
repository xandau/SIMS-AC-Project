using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using WebAPI.AuthServices;
using WebAPI.DTOs;
using WebAPI.Models;

namespace WebAPI.Repository
{
    public class TicketRepository : ARepository<Ticket>, ITicketRepository
    {    
        private JwtService jwtService;
        private readonly IAmazonLambda _lambdaClient;

        public TicketRepository(SIMSContext context, IAmazonLambda client) : base(context)
        {
#if DEBUG
            _context = context;
            jwtService = new JwtService();
#else
            _context = context;
            jwtService = new JwtService();
            _lambdaClient = client;
#endif
        }

        public override async Task<Ticket> GetAsync(long id, string access_token = "")
        {
            Ticket? entity = null;

            if (_entities.Count() == 0)
                return null;

            entity = await _entities.Include(t => t.Creator).FirstAsync(u => u.ID == id);

            if (entity is not null)
            {
                _logEntry.Add(new LogEntry()
                {
                    Level = LogLevel.Information,
                    Timestamp = DateTime.Now,
                    Message = $"Element of type {entity.GetType().Name} with ID \"{entity.ID}\" read"
                });
                await _context.SaveChangesAsync();
            }
            return entity;
        }

        public override async Task<Ticket> CreateAsync(Ticket entity)
        {
            Ticket ticket = new Ticket()
            {
                Title = entity.Title,
                Description = entity.Description,
                State = entity.State,
                CreationTime = DateTime.Now,
                Severity = entity.Severity,
                CVE = entity.CVE,
                CreatorID = entity.CreatorID,
                AssignedPersonID = entity.AssignedPersonID,
                ReferenceID = entity.ReferenceID
            };

            _entities.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<Ticket>> GetAssignedTickets(string access_token)
        {
            long id = jwtService.GetClaimsFromToken(access_token);
            if (id == 0)
                throw new Exception("No Identifier Found");
            else
                return await _entities.Where(t => t.AssignedPersonID == id).ToListAsync();
        }

        public async Task<List<Ticket>> GetCreatedTickets(string access_token)
        {
            long id = jwtService.GetClaimsFromToken(access_token);
            if (id == 0)
                throw new Exception("No Identifier Found");
            else
                return await _entities.Where(t => t.CreatorID == id).ToListAsync();
        }

        public async Task<bool> StopInstance(string access_token, StopInstance request)
        {
            long id = jwtService.GetClaimsFromToken(access_token);
            string instanceId = request.InstanceId;


            if (id == 0)
                throw new Exception("No Identifier Found");
            if (id != request.Id)
                throw new Exception("You do not have permission to stop this instance.");
            else
            {
                Ticket? ticket = _entities.FirstOrDefault(t => t.ReferenceID == instanceId && t.CreatorID == id);

                if (ticket is null)
                    throw new Exception("Ticket not found.");

                var result = await CallLambdaAsync("TerminateInstances", new { instance_id = instanceId });

                ticket.State = Enums.ETicketState.CLOSED;
                _context.SaveChanges();

                return true;
            }
        }

        private async Task<string> CallLambdaAsync(string functionName, object payload)
        {
            var request = new InvokeRequest
            {
                FunctionName = functionName,
                InvocationType = InvocationType.RequestResponse,
                Payload = JsonSerializer.Serialize(payload)
            };

            var response = await _lambdaClient.InvokeAsync(request);

            if (response.StatusCode != 200)
            {
                using var errorReader = new StreamReader(response.Payload);
                var errorPayload = await errorReader.ReadToEndAsync();
                throw new Exception($"Lambda invocation failed. StatusCode: {response.StatusCode}, FunctionError: {response.FunctionError}, Payload: {errorPayload}");
            }

            using var reader = new StreamReader(response.Payload);
            return await reader.ReadToEndAsync(); // Successful response from Lambda
        }
    }
}
