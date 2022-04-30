using System.Net;

namespace desert_auth.Class
{
    public class IPControlMiddleware
    {
        Service _service = new Service(true);
        readonly RequestDelegate _next;
        public IPControlMiddleware(RequestDelegate next)
        {
            
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            IPAddress remoteIp = context.Connection.RemoteIpAddress;
            string request = context.Request.Query.ToString();
            var ips = _service.IPWhiteList;
            foreach (string ip in ips)
            {
                if (!IPAddress.Parse(ip).Equals(remoteIp))
                {
                    Console.WriteLine("[REQUEST DENIED] IP: " + remoteIp);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Authorization Failure");
                    return;
                }
            }


            await _next.Invoke(context);
        }
    }
}
