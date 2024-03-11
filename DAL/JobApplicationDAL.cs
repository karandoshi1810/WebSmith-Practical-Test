using JobListing.Models;
using System.Data.SqlClient;
using System.Data;
using JobListing.ViewModels;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.Mime;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace JobListing.DAL
{
    [Authorize]
    public class JobApplicationDAL
    {
        public HttpContext _httpContext => new HttpContextAccessor().HttpContext;

        public async Task<List<State>> LoadStates()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (var con = new SqlConnection(jobListingDb))
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
                    while (rdr.Read())
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<AreaOfInterestViewModel>> LoadAreaOfInterests()
        {
            List<AreaOfInterestViewModel> areaOfInterests = new List<AreaOfInterestViewModel>();
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(jobListingDb))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("usp_get_area_of_interests", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        areaOfInterests.Add(new AreaOfInterestViewModel
                        {
                            InterestId = (int)rdr["interest_id"],
                            InterestName = (string)rdr["interest_name"],
                            IsChecked = (bool)rdr["is_checked"]
                        });
                    }
                }
                return areaOfInterests;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> InsertUserApplication(IFormCollection userApplication)
        {

            string? userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null )
            {
                return "User not logged in!";
            }
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            IFormFile resume = userApplication.Files[0];
            string contentType = resume.ContentType;
            string fileName = Path.GetFileName(resume.FileName);

            using (MemoryStream msm = new MemoryStream())
            {
                resume.CopyTo(msm);
                try
                {
                    using (SqlConnection con = new SqlConnection(jobListingDb))
                    {
                        SqlCommand cmd = new SqlCommand("usp_insert_application", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@user_id", int.Parse(userId));
                        cmd.Parameters.AddWithValue("@first_name", userApplication["FirstName"].ToString());
                        cmd.Parameters.AddWithValue("@last_name", userApplication["LastName"].ToString());
                        cmd.Parameters.AddWithValue("@email", userApplication["Email"].ToString());
                        cmd.Parameters.AddWithValue("@contact_number", userApplication["ContactNumber"].ToString());
                        cmd.Parameters.AddWithValue("@date_of_birth", userApplication["DateOfBirth"].ToString());
                        cmd.Parameters.AddWithValue("@gender", userApplication["Gender"].ToString());
                        cmd.Parameters.AddWithValue("@state", userApplication["State"].ToString());
                        cmd.Parameters.AddWithValue("@city", userApplication["City"].ToString());
                        cmd.Parameters.AddWithValue("@address", userApplication["Address"].ToString());
                        cmd.Parameters.AddWithValue("@area_of_interest", userApplication["AreaOfInterest"].ToString());
                        cmd.Parameters.AddWithValue("@resume", msm.ToArray());
                        cmd.Parameters.AddWithValue("@content_type", contentType.ToString());
                        cmd.Parameters.AddWithValue("@file_name", fileName.ToString());
                        cmd.Parameters.AddWithValue("@applied_on", DateTime.Now);
                        cmd.Parameters.AddWithValue("@updated_on", DateTime.Now);
                        cmd.Parameters.AddWithValue("is_active", 1);
                        cmd.Parameters.AddWithValue("is_deleted", 0);
                        cmd.Parameters.Add("@return_value", SqlDbType.Int);
                        cmd.Parameters["@return_value"].Direction = ParameterDirection.Output;
                        await con.OpenAsync();
                        await cmd.ExecuteScalarAsync();
                        int returnValue = (int)cmd.Parameters["@return_value"].Value;
                        if(returnValue == 1 )
                        {
                            con.Close();
                            return "Application inserted successfully";
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return "Internal Server Error";
        }

        public async Task<List<JobApplication>> LoadApplications()
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                var jobListingDb = configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(jobListingDb))
                {
                    SqlCommand cmd = new SqlCommand("usp_get_all_applications", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    List<JobApplication> applications = new List<JobApplication>();
                    while (rdr.Read())
                    {
                        applications.Add(new JobApplication
                        {
                            ApplicationId = (int)rdr["application_id"],
                            FirstName = rdr["first_name"].ToString(),
                            LastName = rdr["last_name"].ToString(),
                            Email = rdr["email"].ToString(),
                            ContactNumber = rdr["contact_number"].ToString(),
                            DateOfBirth = (DateTime)rdr["date_of_birth"],
                            Gender = rdr["gender"].ToString(),
                            State = rdr["state"].ToString(),
                            City = rdr["state"].ToString(),
                            Address = rdr["address"].ToString(),
                            AreaOfInterest = rdr["area_of_interest"].ToString().Split(','),
                            FileName = rdr["file_name"].ToString(),
                            ContentType = rdr["content_type"].ToString(),
                            Resume = (byte[])rdr["resume"]
                        });
                    }
                    return applications;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JobApplication> GetUserApplication([FromRoute(Name = "applicationId")] int applicationId)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            JobApplication userApplication = new JobApplication();
            try
            {
                using(SqlConnection con = new SqlConnection(jobListingDb))
                {
                    
                    SqlCommand cmd = new SqlCommand("usp_get_user_application", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("application_id", applicationId);
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    rdr.Read();
                    userApplication.ApplicationId = applicationId;
                    userApplication.FirstName = rdr["first_name"].ToString();
                    userApplication.LastName = rdr["last_name"].ToString();
                    userApplication.Email = rdr["email"].ToString();
                    userApplication.ContactNumber = rdr["contact_number"].ToString();
                    userApplication.DateOfBirth = (DateTime)rdr["date_of_birth"];
                    userApplication.Gender = rdr["gender"].ToString();
                    userApplication.State = rdr["state"].ToString();
                    userApplication.City = rdr["city"].ToString();
                    userApplication.Address = rdr["address"].ToString();
                    userApplication.AreaOfInterest = rdr["area_of_interest"].ToString().Split(',');
                    userApplication.FileName = rdr["file_name"].ToString();
                    userApplication.ContentType = rdr["content_type"].ToString();
                    userApplication.Resume = (byte[])rdr["resume"];
                    userApplication.AppliedOn = (DateTime)rdr["applied_on"];
                    userApplication.Updatedon = (DateTime)rdr["updated_on"];
                    userApplication.IsActive = Convert.ToBoolean(rdr["is_active"]);
                    userApplication.IsDeleted = Convert.ToBoolean(rdr["is_deleted"]);
                }
                return userApplication; 
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FileDetailViewModel> GetResumeFile(int applicationId)
        {
            FileDetailViewModel fileDetail= new FileDetailViewModel();
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                var jobListingDb = configuration.GetConnectionString("DefaultConnection");
                using(SqlConnection con = new SqlConnection(jobListingDb))
                {
                    SqlCommand cmd = new SqlCommand("usp_get_user_resume_file", con);
                    cmd.Parameters.AddWithValue("@application_id", applicationId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    rdr.Read();
                    fileDetail.FileName = rdr["file_name"].ToString();
                    fileDetail.ContentType = rdr["content_type"].ToString();
                    fileDetail.Content = (byte[])rdr["resume"];
                    return fileDetail;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<string> EditApplication(IFormCollection updatedUserApplication)
        {
            string? userId = _httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return "User not logged in!";
            }
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            IFormFile updatedResume = null;
            FileDetailViewModel currentResume = new FileDetailViewModel();
            string contentType = String.Empty;
            string fileName = String.Empty;
            if (updatedUserApplication.Files.Count() > 0)
            {
                updatedResume = updatedUserApplication.Files[0];
                contentType = updatedResume.ContentType;
                fileName = Path.GetFileName(updatedResume.FileName);
            }
            else
            {
                currentResume = await GetResumeFile(int.Parse(updatedUserApplication["ApplicationId"].ToString()));
                //var currentResume = updatedUserApplication["Resume"].ToString();
                var currentResume2 = System.Text.Encoding.ASCII.GetBytes(updatedUserApplication["Resume"]);
                var type = updatedUserApplication["Resume"].GetType();
                //contentType = currentResume.ContentType;
                //fileName = resume.FileName;
            }
            using (MemoryStream msm = new MemoryStream())
            {
                if (updatedUserApplication.Files.Count() > 0)
                {
                    updatedResume = updatedUserApplication.Files[0];
                    contentType = updatedResume.ContentType;
                    fileName = Path.GetFileName(updatedResume.FileName);
                    updatedResume.CopyTo(msm);
                }
                try
                {
                    using (SqlConnection con = new SqlConnection(jobListingDb))
                    {
                        SqlCommand cmd = new SqlCommand("usp_update_application", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@application_id", int.Parse(updatedUserApplication["ApplicationId"]));
                        cmd.Parameters.AddWithValue("@user_id", int.Parse(userId));
                        cmd.Parameters.AddWithValue("@first_name", updatedUserApplication["FirstName"].ToString());
                        cmd.Parameters.AddWithValue("@last_name", updatedUserApplication["LastName"].ToString());
                        cmd.Parameters.AddWithValue("@email", updatedUserApplication["Email"].ToString());
                        cmd.Parameters.AddWithValue("@contact_number", updatedUserApplication["ContactNumber"].ToString());
                        cmd.Parameters.AddWithValue("@date_of_birth", updatedUserApplication["DateOfBirth"].ToString());
                        cmd.Parameters.AddWithValue("@gender", updatedUserApplication["Gender"].ToString());
                        cmd.Parameters.AddWithValue("@state", updatedUserApplication["State"].ToString());
                        cmd.Parameters.AddWithValue("@city", updatedUserApplication["City"].ToString());
                        cmd.Parameters.AddWithValue("@address", updatedUserApplication["Address"].ToString());
                        cmd.Parameters.AddWithValue("@area_of_interest", updatedUserApplication["AreaOfInterest"].ToString());
                        if(updatedUserApplication.Files.Count() > 0)
                        {
                            cmd.Parameters.AddWithValue("@resume", msm.ToArray());
                            cmd.Parameters.AddWithValue("@content_type", contentType.ToString());
                            cmd.Parameters.AddWithValue("@file_name", fileName.ToString());
                        }
                        else
                        {
                            //cmd.Parameters.Add("@resume", SqlDbType.VarBinary, -1).Value = System.Text.Encoding.ASCII.GetBytes(updatedUserApplication["Resume"].ToString());
                            //cmd.Parameters.AddWithValue("@resume", System.Text.Encoding.ASCII.GetBytes(updatedUserApplication["Resume"].ToString()));
                            cmd.Parameters.AddWithValue("@resume", currentResume.Content);
                            cmd.Parameters.AddWithValue("@content_type", currentResume.ContentType);
                            cmd.Parameters.AddWithValue("@file_name", currentResume.FileName);
                        }
                        cmd.Parameters.AddWithValue("@applied_on", updatedUserApplication["AppliedOn"].ToString());
                        cmd.Parameters.AddWithValue("@updated_on", DateTime.Now);
                        cmd.Parameters.Add("@is_active", SqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("@is_deleted", SqlDbType.Bit).Value= false;
                        cmd.Parameters.Add("@return_value", SqlDbType.Int);
                        cmd.Parameters["@return_value"].Direction = ParameterDirection.Output;
                        await con.OpenAsync();
                        await cmd.ExecuteScalarAsync();
                        int returnValue = (int)cmd.Parameters["@return_value"].Value;
                        if (returnValue == 1)
                        {
                            con.Close();
                            return "Application updated successfully";
                        }
                        con.Close();
                        if (returnValue == 0)
                        {
                            return "Internal Server Error";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return "Internal Server Error";
            }
        }

        public async Task<string> DeleteApplication(int applicationId)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                var jobListingDb = configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(jobListingDb))
                {
                    SqlCommand cmd = new SqlCommand("usp_delete_user_application", con);
                    cmd.Parameters.AddWithValue("@application_id", applicationId);
                    cmd.Parameters.Add("@return_value", SqlDbType.Int);
                    cmd.Parameters["@return_value"].Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    await cmd.ExecuteScalarAsync();
                    int returnValue = (int)cmd.Parameters["@return_value"].Value;
                    if (returnValue == 1)
                    {
                        con.Close();
                        return "Application deleted successfully";
                    }
                    con.Close();
                    if (returnValue == 0)
                    {
                        return "Internal Server Error";
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return "Internal Server Error";
        }

    }
}
