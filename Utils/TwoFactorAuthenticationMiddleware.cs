namespace BlazorVault.Utils
{
    public class TwoFactorAuthenticationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext httpContext)
        {
            var isSSOAuthenticated = httpContext.Session.GetString("SSOAuthenticated") == "true";
            var is2FAAuthenticated = httpContext.Session.GetString("2FAAuthenticated") == "true";
            if (isSSOAuthenticated && !is2FAAuthenticated)
            {
                httpContext.Response.Redirect("/master-password");
                return;
            }

            await _next(httpContext);
        }
    }

}
