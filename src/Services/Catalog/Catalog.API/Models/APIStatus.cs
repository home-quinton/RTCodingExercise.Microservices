using System.Net;

namespace Catalog.API.Models;

public class APIStatus
{
    public int Status { get; set; }
    public string Message { get; set; }

    public APIStatus()
    {

    }

    public APIStatus(HttpStatusCode status, string msg)
    {
        Status = (int)status;
        Message = msg;
    }
}
 
