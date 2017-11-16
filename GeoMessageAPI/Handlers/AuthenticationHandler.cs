using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GeoMessageAPI.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = request.Headers.GetValues("Authorization").First();

            if (authorizationHeader != null && authorizationHeader.StartsWith("Basic", StringComparison.InvariantCultureIgnoreCase))
            {
                string authorizationUserAndPwdBase64 = authorizationHeader.Substring("Basic ".Length);
                string authorizationuserAndPwd = Encoding.Default.GetString(Convert.FromBase64String(authorizationUserAndPwdBase64));
                string user = authorizationuserAndPwd.Split(':')[0];
                string password = authorizationuserAndPwd.Split(':')[1];

                if (user == password)
                {
                    
                }
                else
                    Unauthorized();
            }
            else
                Unauthorized();

            return base.SendAsync(request, cancellationToken);

        }



        private Task<HttpResponseMessage> Unauthorized()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

    }
}
