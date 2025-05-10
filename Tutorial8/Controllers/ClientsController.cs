using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        // Ten endpoint będzie pobierał wszystkie wycieczki powiązane z konkretnym klientem:
        [HttpGet("{id:int}/trips")] 
        public async Task<IActionResult> GetClientTrips(int id)
        {
            if (await _clientsService.DoesClientExist(id) == false)
            {
                return NotFound("Client not found!");
            }
            
            var trips = await _clientsService.GetClientTrips(id);

            if (trips.Count == 0)
            {
                return NotFound("Trips not found");
            }
            
            return Ok(trips);
        }

        // Ten endpoint dodaje klienta:
        [HttpPost] 
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO client)
        {
            if (await _clientsService.CreateClient(client))
            {
                return Ok("Client created!");
            }
            return BadRequest("Failed to create client");
        }

        // Ten endpoint zarejestruje klienta na konkretną wycieczkę
        [HttpPut("{id:int}/trips/{tripId:int}")]
        public async Task<IActionResult> RegisterClientTrip(int id, int tripId)
        {
            if (await _clientsService.DoesClientExist(id) == false)
            {
                return NotFound("Client not found");
            }

            if (await _clientsService.DoesTripExist(tripId) == false)
            {
                return NotFound("Trip not found");
            }

            if (await _clientsService.DoesRegistrationExist(id, tripId))
            {
                return Conflict("Client already registered for the trip!");
            }

            if (await _clientsService.IsTripNotFull(tripId) == false)
            {
                return Conflict("Trip is full!");
            }

            if (await _clientsService.RegisterClientTrip(id, tripId))
            {
                return Ok("Client registered for the trip!");
            }
            
            return BadRequest("Failed to register client");
        }

        // Ten endpoint usuwa rejestrację klienta na wycieczkę
        [HttpDelete("{id:int}/trips/{tripId:int}")]
        public async Task<IActionResult> UnregisterClientTrip(int id, int tripId)
        {
            if (await _clientsService.DoesRegistrationExist(id, tripId) == false)
            {
                return NotFound("Registration not found");
            }

            if (await _clientsService.UnregisterClientTrip(id, tripId))
            {
                return Ok("Client unregistered from the trip!");
            }
            
            return BadRequest("Failed to unregister client");
        }
    }
}