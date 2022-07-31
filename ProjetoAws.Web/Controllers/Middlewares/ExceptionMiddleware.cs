namespace ProjetoAws.Web.Controllers.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (System.Exception ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new {Msg = ex.Message});
            }
        }
    }
}