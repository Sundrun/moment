using System.Net;
using MomentApi.CreateUser;

namespace MomentApi.Helpers;

public static class ToStatusCodeHelper
{
    public static HttpStatusCode ToStatusCode(ICreateUserResponse response) =>
        response switch
        {
            UserCreated => HttpStatusCode.Created,
            UserExists => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };
}