using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace matensa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferFundsController : ControllerBase
    {
        [HttpGet]
        [Route("transfer")]
        public void TransferFunds()
        {

        }
    }
}
