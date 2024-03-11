using JobListing.Models;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace JobListing.DAL
{
    public class StateAndCityDAL
    {
        public async Task<List<State>> LoadStates()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
               using(var con = new SqlConnection(jobListingDb))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("usp_get_all_states", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    List<State> states = new List<State>();
                    while (rdr.Read())
                    {
                        states.Add(new State
                        {
                            StateId = (int)rdr["state_id"],
                            StateName = (string)rdr["state"],

                        });
                    }
                    return states;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<City>> LoadCities(string StateId)
        {
            var cities = new List<City>();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var con = new SqlConnection(jobListingDb))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("usp_get_cites_of_state", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@state_id", StateId);
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while(rdr.Read())
                    {
                        cities.Add(new City
                        {
                            CityId = (int)rdr["city_id"],
                            CityName = (string)rdr["city"],
                            StateId = (int)rdr["state_id"],

                        });
                    }
                }
                return cities;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
