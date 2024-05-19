using Azure;
using matensa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Response = matensa.Models.Response;

namespace matensa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public UsersController(IConfiguration configurtion)
        {
            _configuration = configurtion;
        }

        //private static List<string> ListUsers = new List<string>() { "rami", "ahmad" };

        [HttpGet]
        [Route("List")]
        public string GetAllUsers()
        {

            SqlConnection con = new SqlConnection( _configuration.GetConnectionString("MatensaAppCon").ToString()  );
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.Users",con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<User> UsersList = new List<User> ();
            Response response = new Response();
            if(dt.Rows.Count > 0 )
            {
                for( int i = 0; i < dt.Rows.Count; i++ )
                {

                    User user = new User();
                    user.Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                    user.Name = Convert.ToString(dt.Rows[i]["Name"]);
                    user.Email = Convert.ToString(dt.Rows[i]["Email"]);
                    user.Phone = Convert.ToString(dt.Rows[i]["Phone"]);
                    user.Dob = Convert.ToString(dt.Rows[i]["Dob"]);
                    user.Password = Convert.ToString(dt.Rows[i]["Password"]);
                    UsersList.Add(user);
                }
            }
            if(UsersList.Count> 0)
            {
               return JsonConvert.SerializeObject(UsersList);
            } else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "No data found.";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpGet("{id}")]
        public string GetOneUser(int id)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.Users WHERE id = " + id, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Response response = new Response();
            if(dt.Rows.Count > 0)
            {
                int i = 0;
                User user = new User();
                user.Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                user.Name = Convert.ToString(dt.Rows[i]["Name"]);
                user.Email = Convert.ToString(dt.Rows[i]["Email"]);
                user.Phone = Convert.ToString(dt.Rows[i]["Phone"]);
                user.Dob = Convert.ToString(dt.Rows[i]["Dob"]);
                user.Password = Convert.ToString(dt.Rows[i]["Password"]);
                user.Balance = Convert.ToDecimal(dt.Rows[i]["Balance"]);
                return JsonConvert.SerializeObject(user); ;
            } else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "User is not exist.";
                return JsonConvert.SerializeObject(response);
            }

        }

        [HttpPost]
        [Route("Create")]

        public string AddNewUser(User user)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            bool validEmail = user.IsValid(user.Email);
            Response response = new Response();
            if (validEmail == false)
            {
                response.StatusCode = 100;
                response.ErrorMessage = "Invalid Email.";
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                string usersAll = GetAllUsers();
                List<User> usersList = JsonConvert.DeserializeObject<List<User>>(usersAll);
                foreach (User u in usersList)
                {
                    if (u.Email == user.Email)
                    {
                        response.StatusCode = 100;
                        response.ErrorMessage = "Email already exist.";
                        return JsonConvert.SerializeObject(response);
                    }
                }
                //string password = user.EncodePasswordToBase64(user.Password);
                string password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                string token = CreateToken(user);
                SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Users (Name, Email,Phone, Dob, Password, Token, Balance) VALUES ('" + user.Name + "', '" + user.Email + "', '" + user.Phone + "', '" + user.Dob + "', '" + password + "', '" + token + "', '0')", con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return "User was added successfully.";
                }
                else
                { 
                    response.StatusCode = 100;
                    response.ErrorMessage = "Error Occur.";
                    return JsonConvert.SerializeObject(response);
                }
            }
            
        }

        [HttpPost]
        [Route("Balance/{id:int}")]
        public string AddBalance(int id, float balance) {
  
            
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            SqlCommand cmd = new SqlCommand("Update dbo.Users SET Balance = '" + balance + "' WHERE Id = "+id, con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            Response response = new Response();
            if (i  > 0)
            {
                return "Balance was added successfully.";
            } else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "Error Occur.";
                return JsonConvert.SerializeObject(response);
            }
            

        }
       
        [HttpPut("{id}")]
        //[Route("Update")]
        public string Update(int id, User user)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            bool validEmail = user.IsValid(user.Email);
            Response response = new Response();
            if (validEmail == false)
            {
                response.StatusCode = 100;
                response.ErrorMessage = "Invalid Email.";
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                string usersAll = GetAllUsers();
                List<User> usersList = JsonConvert.DeserializeObject<List<User>>(usersAll);
                foreach (User u in usersList)
                {
                    if (u.Email == user.Email && u.Id != id)
                    {
                        response.StatusCode = 100;
                        response.ErrorMessage = "Email already exist.";
                        return JsonConvert.SerializeObject(response);
                    }
                }
                string password = user.EncodePasswordToBase64(user.Password);
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET Name = '" + user.Name + "', Email = '" + user.Email + "',Phone =  '" + user.Phone + "', Dob = '" + user.Dob + "', Password = '" + password + "' WHERE Id = " + id, con);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    return "User was updated successfully.";
                }
                else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "Error Occur.";
                    return JsonConvert.SerializeObject(response);
                }
            }

        }
        

        [HttpDelete("{id}")]
        //[Route("DeleteUser")]
        public string Delete(int id)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            SqlCommand cmd = new SqlCommand("Delete FROM dbo.Users WHERE Id = " + id, con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i > 0)
            {
                return "User was deleted successfully.";
            }
            else
            {
                return "Error Occur.";
            }
        }


        [HttpPost]
        [Route("Login")]
        public string Login(string email, string password)
        {
            User user = new User();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.Users WHERE Email = '"+email+"' ", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Response response = new Response();
            if (dt.Rows.Count > 0) {

                //string dec_password = user.DecodeFrom64(Convert.ToString(dt.Rows[0]["Password"]));
                //if(dec_password == password) {
                if (BCrypt.Net.BCrypt.Verify(password, Convert.ToString(dt.Rows[0]["Password"]))) { 
                        return JsonConvert.SerializeObject(Convert.ToString(dt.Rows[0]["Token"]));
                } else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "Check email or passwod.";
                    return JsonConvert.SerializeObject(response);

                }
            } else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "User is not exist.";
                return JsonConvert.SerializeObject(response);
            }
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.Name)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        /*
        [HttpGet]
        public IActionResult GetUsers()
        {
            if (ListUsers.Count > 0)
            {
                return Ok(ListUsers);
            } else
            {
                return NoContent();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id) { 
            if( id >= 0 && id < ListUsers.Count ){
                return Ok(ListUsers[id]);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult AddUser(string username)
        {
            ListUsers.Add(username);
                return Ok();

        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, string username)
        {
            if(id >= 0 && id < ListUsers.Count)
            {
                ListUsers[id] = username;
            }
            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id >= 0 && id < ListUsers.Count)
            {
                ListUsers.RemoveAt(id);
            }
            return NoContent();

        }
        */
    }
}
