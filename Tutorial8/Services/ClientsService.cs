using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=sa; Password=yourStrong(!)Password; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

    public async Task<List<TripDTO>> GetClientTrips(int idClient) // GET/api/clients/{id}/trips
    {
        var trips = new List<TripDTO>();
        
        // Zapytanie SQL pobierające wszystkie wycieczki klienta o konkretnym id:
        string command = @"SELECT Trip.IdTrip, Trip.Name FROM Trip 
                    INNER JOIN Client_Trip ON Client_Trip.IdTrip = Trip.IdTrip
                    WHERE Client_Trip.IdClient = @idClient"; 
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@idClient", idClient);
            
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1)
                    });
                }
            }
        }

        return trips;
    }

    public async Task<bool> CreateClient(ClientDTO client) // POST /api/clients
    {
        // Zapytanie SQL, które dodaje nowy wiersz w tabeli Client:
        string command = @"insert into Client (FirstName, LastName, Email, Telephone, Pesel)
                            values (@FirstName, @LastName, @Email, @Telephone, @Pesel)"; 
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            
            await conn.OpenAsync();
            
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public async Task<bool> RegisterClientTrip(int idClient, int idTrip) // PUT /api/clients/{id}/trips{tripId}
    {
        // Zapytanie SQL dodające wiersz w tabeli Client_Trip, aby zarejestrować klienta na wycieczkę
        string command = @"INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                          VALUES (@IdClient, @IdTrip, @RegisteredAt, NULL)";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            cmd.Parameters.AddWithValue("@RegisteredAt", int.Parse(DateTime.Now.ToString("yyyyMMdd")));
            
            await conn.OpenAsync();
            
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }

    public async Task<bool> IsTripNotFull(int idTrip)
    {
        // Zapytanie sql znajdujące liczbę osób zarejestrowanych na konkretną wycieczkę i max. liczbę uczestników:
        string command = @"SELECT Trip.MaxPeople, COUNT(Client_Trip.IdClient) AS Registered FROM Trip
                          LEFT JOIN Client_Trip ON Client_Trip.IdTrip = Trip.IdTrip
                          WHERE Trip.IdTrip = @IdTrip GROUP BY Trip.MaxPeople";

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);

            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    int maxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople"));
                    int registeredClients = reader.GetInt32(reader.GetOrdinal("Registered"));
                    return registeredClients < maxPeople;
                }

                return false;
            }
        }
    }

    public async Task<bool> UnregisterClientTrip(int idClient, int idTrip) // DELETE /api/clients/{id}/trips/{tripId}
    {
        // Zapytanie SQL usuwające wiersz z tabeli Client_Trip - usunięcie rejestracji klienta:
        string command = @"DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            
            await conn.OpenAsync();
            
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
    
    public async Task<bool> DoesClientExist(int idClient)
    {
        // Zapytanie SQL mające znaleźć klienta o danym Id:
        string command = "SELECT COUNT(*) FROM Client WHERE IdClient = @IdClient";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            
            await conn.OpenAsync();
            
            int count = (int) await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }
    
    public async Task<bool> DoesTripExist(int idTrip)
    {
        // Zapytanie SQL mające znaleźć wycieczkę o danym Id:
        string command = "SELECT COUNT(*) FROM Trip WHERE IdTrip = @IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            
            await conn.OpenAsync();
            
            int count = (int) await cmd.ExecuteScalarAsync();
            return count > 0;
        }
    }

    public async Task<bool> DoesRegistrationExist(int idClient, int idTrip)
    {
        // Zapytanie sprawdzające czy rejestracja klienta na daną wycieczkę istnieje:
        string command = "SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdClient", idClient);
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            await conn.OpenAsync();
            
            var result = await cmd.ExecuteScalarAsync();
            if (result != null)
                return (int)result > 0;
            return false;
        }
    }
    
}