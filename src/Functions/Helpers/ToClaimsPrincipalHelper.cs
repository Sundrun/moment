// using System.Net;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.Azure.Functions.Worker.Http;
// using Microsoft.IdentityModel.Tokens;
//
// namespace Functions.Helpers;
//
// // TODO
// public static class ToClaimsPrincipalHelper
// {
//     public static bool TryConvertToClaimsPrincipal(HttpRequestData request, out ClaimsPrincipal principal)
//     {
//         if (!request.Headers.TryGetValues("Authorization", out var authHeaderValues))
//         {
//             principal = new ClaimsPrincipal();
//             return false;
//         }
//         
//         var authHeader = authHeaderValues.FirstOrDefault();
//         if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//         {
//             principal = new ClaimsPrincipal();
//             return false;
//         }
//         
//         var jwtToken = authHeader.Substring("Bearer ".Length).Trim();
//         
//         var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
//         try
//         {
//             var tokenValidationParameters = new TokenValidationParameters
//             {
//                 ValidateIssuerSigningKey = true,
//                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey")), // Replace with your actual key
//                 ValidateIssuer = true,
//                 ValidIssuer = "YourIssuer", // Replace with your actual issuer
//                 ValidateAudience = true,
//                 ValidAudience = "YourAudience", // Replace with your actual audience
//                 ValidateLifetime = true,
//                 ClockSkew = TimeSpan.Zero // No clock skew tolerance
//             };
//
//             SecurityToken validatedToken;
//             var principal = await tokenHandler.ValidateTokenAsync(jwtToken, tokenValidationParameters);
//
//             // Access claims from the principal object
//             var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             // ...
//         }
//         catch (SecurityTokenExpiredException)
//         {
//             var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
//             await unauthorizedResponse.WriteStringAsync("Token has expired.");
//             return unauthorizedResponse;
//         }
//         catch (Exception ex)
//         {
//             var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
//             await unauthorizedResponse.WriteStringAsync($"Token validation failed: {ex.Message}");
//             return unauthorizedResponse;
//         }
//         
//         
//         
//         var identity = request.Identities.First();
//         principal = new ClaimsPrincipal(identity);
//         return true;
//     }
// }