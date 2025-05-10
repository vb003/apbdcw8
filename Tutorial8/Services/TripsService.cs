using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=sa; Password=yourStrong(!)Password; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
    
    public async Task<List<TripDTO>> GetTrips() // GET/api/trips
    {
        var trips = new List<TripDTO>();
        
        // Zapytanie SQL pobierające wszystkie wycieczki wraz z podstawowymi informacjami:
        string command = "SELECT IdTrip, Name, Description, DateFrom, DateTo, maxPeople FROM Trip"; 
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int idTrip = reader.GetInt32(idOrdinal);
                    trips.Add(new TripDTO()
                    {
                        Id = idTrip,
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        maxPeople = reader.GetInt32(5),
                        Countries = await GetCountries(idTrip)
                    });
                }
            }
        }
        
        return trips;
    }

    private async Task<List<CountryDTO>> GetCountries(int idTrip)
    {
        List<CountryDTO> countries = new List<CountryDTO>();
        
        // Zapytanie SQL wyszukujące kraje przypisane do wycieczki o danym Id:
        string command = @"SELECT Name FROM Country
                            INNER JOIN Country_Trip on Country_Trip.IdCountry = Country.IdCountry
                            WHERE Country_Trip.IdTrip = @IdTrip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@IdTrip", idTrip);
            
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    countries.Add(new CountryDTO()
                    {
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    });
                }
            }
        }
        return countries;
    }
}