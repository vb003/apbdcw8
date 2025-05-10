using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripDTO>> GetClientTrips(int idClient);
    Task<bool> CreateClient(ClientDTO client);
    Task<bool> RegisterClientTrip(int idClient, int idTrip);
    Task<bool> UnregisterClientTrip(int idClient, int idTrip);
    Task<bool> DoesClientExist(int idClient);
    Task<bool> DoesTripExist(int idTrip);
    Task<bool> DoesRegistrationExist(int idClient, int idTrip);
}