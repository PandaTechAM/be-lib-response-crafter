namespace ResponseCrafter.Demo;

public static class HttpContextExtensions
{
    public static string GetToken(this HttpContext httpContext)
    {
        return httpContext.Response.Headers.Authorization.ToString();
    }

    public static void SetToken(this HttpContext httpContext, string token)
    {
        httpContext.Response.Headers.Authorization = token;
    }
}