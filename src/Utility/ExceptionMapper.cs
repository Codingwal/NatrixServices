using System.Net;

namespace NatrixServices;

public static class ExceptionMapper
{
    public static HttpStatusCode GetStatusCode(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,

            IBadRequestException => HttpStatusCode.BadRequest,
            INotFoundException => HttpStatusCode.NotFound,
            IConflictException => HttpStatusCode.Conflict,
            IGoneException => HttpStatusCode.Gone,
            IForbiddenException => HttpStatusCode.Forbidden,
            IUnauthorizedException => HttpStatusCode.Unauthorized,
            IUnprocessableException => HttpStatusCode.UnprocessableEntity,

            _ => HttpStatusCode.InternalServerError
        };
    }
}