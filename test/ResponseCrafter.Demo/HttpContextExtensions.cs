namespace ResponseCrafter.Demo;

public static class HttpContextExtensions
{
    extension(HttpContext httpContext)
    {
       public string GetToken()
       {
          return httpContext.Response.Headers.Authorization.ToString();
       }

       public void SetToken(string token)
       {
          httpContext.Response.Headers.Authorization = token;
       }
    }
}