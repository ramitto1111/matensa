using Azure;
using matensa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Response = matensa.Models.Response;
using System.IdentityModel.Tokens.Jwt;

namespace matensa.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TransferFundsController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public TransferFundsController(IConfiguration configurtion)
        {
            _configuration = configurtion;
        }



  


        //[Authorize]
        [HttpPost]
        //[Route("transfer/{id:int}")]
        public string TransferFunds(TransferFund tf)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("MatensaAppCon").ToString());
            

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            // Get the Balance of the user:
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM dbo.Users WHERE Token = '" + token + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                decimal amount = Convert.ToDecimal(dt.Rows[0]["Balance"]);
                int id = Convert.ToInt32(dt.Rows[0]["Id"]);
                if (amount >= 0 && amount >= tf.Amount && tf.Receiver_id != 0 && tf.Amount != 0)
                {
                    // Inser into Transfer funds table.
                    SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Transfer_funds (User_id, Receiver_id,Amount) VALUES ('" + id + "', '" + tf.Receiver_id + "', '" + tf.Amount + "')", con);
                    con.Open();
                    int i = cmd.ExecuteNonQuery();
                    
                    if (i > 0)
                    {
                        decimal new_amount = amount - tf.Amount;
                        // update the balance of the user.
                        SqlCommand cmd1 = new SqlCommand("UPDATE dbo.Users SET Balance = " + new_amount +" WHERE Id = "+id, con);
                        int j = cmd1.ExecuteNonQuery();

                        

                        return "Amount was transfered successfully.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.ErrorMessage = "Error Occur.";
                        return JsonConvert.SerializeObject(response);
                    }
                    con.Close();
                } else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "Amount is not available.";
                    return JsonConvert.SerializeObject(response);
                }
            } else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "User is not available.";
                return JsonConvert.SerializeObject(response);
            }
            

        }



        private string GetUserIdFromToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null) return null;

            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return userIdClaim;
        }
    }
}
