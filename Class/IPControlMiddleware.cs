using System.Net;
using EasMe;

namespace desert_auth.Class
{
    public class IPControlMiddleware
    {
        readonly RequestDelegate _next;
        public IPControlMiddleware(RequestDelegate next)
        {

            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            IPAddress remoteIp = context.Connection.RemoteIpAddress;
            string request = context.Request.Query.ToString();
            var ips = Service.IPWhitelist;
            foreach (string ip in ips)
            {
                if (!IPAddress.Parse(ip).Equals(remoteIp))
                {
                    EasLog.Error("Request denied => IP: " + remoteIp);
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Authorization Failure");
                    return;
                }
            }

            EasLog.Info($"Request {context.Request?.Method} {context.Request.Path.ToUriComponent()}{context.Request?.QueryString.Value} => {context.Response?.StatusCode}");
            await _next(context);            
            await _next.Invoke(context);
        }
       
    }
}
