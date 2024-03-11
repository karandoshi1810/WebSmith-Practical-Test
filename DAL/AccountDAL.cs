using JobListing.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JobListing.DAL
{
    public class AccountDAL
    {
        //private IConfiguration Configuration;

        public AccountDAL()
        {
        }

        public async Task<Tuple<string,string>> Login(Login user)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(jobListingDb))
                {
                    await con.OpenAsync();
                    SqlCommand cmd = new SqlCommand("usp_get_user_details", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@email", user.Email);

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    if (rdr != null)
                    {
                        User userDetails = new User();
                        while (rdr.Read())
                        {
                            userDetails.Email = rdr["email"].ToString();
                            userDetails.Password = rdr["password"].ToString();
                            userDetails.Salt = rdr["salt"].ToString();
                            userDetails.UserId = (int)rdr["user_id"];
                        }
                        byte[] salt = System.Convert.FromBase64String(userDetails.Salt);
                        string enteredPassword = user.Password;
                        bool validUser = validateUser(userDetails.Password, enteredPassword, salt);
                        if (validUser)
                        {
                            return Tuple.Create("User verified successfully",userDetails.UserId.ToString());
                        }
                        else
                        {
                            throw new Exception("Incorrect username or password");
                        }
                    }
                    else
                    {
                        throw new Exception("User not found! Please sign up with us first.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool validateUser(string userPassword, string enteredPassword, byte[] salt)
        {
            const int keySize = 16;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                        Encoding.UTF8.GetBytes(enteredPassword),
                        salt,
                        iterations,
                        hashAlgorithm,
                        keySize
                       );
            string encryptedPassword = Convert.ToHexString(hash);
            if(encryptedPassword == userPassword)
            {
                return true;
            }
            return false;
        }

        public async Task<string> Register(Register user)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var jobListingDb = configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (SqlConnection con = new SqlConnection(jobListingDb))
                {
                    string password = user.Password;
                    PasswordAndSalt passwordAndSalt = EncryptPassword(password);
                    SqlCommand cmd = new SqlCommand("usp_insert_user",con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@first_name", user.FirstName);
                    cmd.Parameters.AddWithValue("@last_name", user.LastName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", passwordAndSalt.Password);
                    cmd.Parameters.AddWithValue("@salt", passwordAndSalt.Salt);
                    cmd.Parameters.AddWithValue("@created_on", DateTime.Now);
                    cmd.Parameters.Add("@return_value", SqlDbType.Int);
                    cmd.Parameters["@return_value"].Direction = ParameterDirection.Output;
                    await con.OpenAsync();
                    await cmd.ExecuteScalarAsync();
                    int returnValue = (int)cmd.Parameters["@return_value"].Value;
                    if(returnValue == 0)
                    {
                            con.Close();
                            throw new Exception("User with same email already exists!. Please register using another email.");
                    }
                    con.Close();
                    return "User created successfully";

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PasswordAndSalt EncryptPassword(string password)
        {
            const int keySize = 16;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            byte[] salt = RandomNumberGenerator.GetBytes(keySize);

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                        Encoding.UTF8.GetBytes(password),
                        salt,
                        iterations,
                        hashAlgorithm,
                        keySize
                       );
            string encryptedPassword = Convert.ToHexString(hash);
            PasswordAndSalt passwordAndSalt = new PasswordAndSalt();
            passwordAndSalt.Salt = System.Convert.ToBase64String(salt);
            passwordAndSalt.Password = encryptedPassword;
            return passwordAndSalt;
        }
    }

}
