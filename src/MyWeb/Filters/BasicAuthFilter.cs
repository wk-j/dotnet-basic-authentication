using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyWeb {
    public class BasicAuthFilter : IAuthorizationFilter {
        private readonly ILogger<BasicAuthFilter> _logger;

        public BasicAuthFilter(ILogger<BasicAuthFilter> logger) {
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            try {
                var header = context.HttpContext.Request.Headers["Authorization"];
                if (header.Count != 0) {
                    var basic = header[0];
                    var headerValue = AuthenticationHeaderValue.Parse(basic);
                    if (headerValue.Scheme == AuthenticationSchemes.Basic.ToString()) {
                        var str = Convert.FromBase64String(headerValue.Parameter ?? string.Empty);
                        var creds = Encoding.UTF8.GetString(str).Split(':', 2);
                        var user = creds[0];
                        var password = creds[1];
                        var ok = IsAuthorized(context, user, password);
                        if (ok) {
                            return;
                        }
                    }
                }
            } catch (Exception ex) {
                context.Result = new UnauthorizedResult();

                _logger.LogError(ex.ToString());
            }

            context.Result = new UnauthorizedResult();
        }

        private bool IsAuthorized(AuthorizationFilterContext context, string user, string password) {
            var userService = context.HttpContext.RequestServices.GetRequiredService<UserService>();
            return userService.IsValidUser(user, password);
        }
    }
}