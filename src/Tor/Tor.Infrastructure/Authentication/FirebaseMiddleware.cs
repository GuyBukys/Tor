using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tor.Infrastructure.Authentication;

internal sealed class FirebaseMiddleware : IMiddleware
{
    private readonly FirebaseSettings _firebaseSettings;
    private readonly HttpClient _googleApiClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<FirebaseMiddleware> _logger;

    public FirebaseMiddleware(
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<FirebaseSettings> firebaseSettings,
        IMemoryCache cache,
        ILogger<FirebaseMiddleware> logger)
    {
        _googleApiClient = httpClientFactory.CreateClient(FirebaseConstants.GoogleApiClient);
        _firebaseSettings = firebaseSettings.Value;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation("starting to validate request for appcheck token");

        if (!_firebaseSettings.IsFirebaseEnabled)
        {
            _logger.LogInformation("firebase middleware disabled. skipping authentication.");
            await next(context);
            return;
        }

        bool isHeaderExists = context.Request.Headers.TryGetValue(FirebaseConstants.FirebaseAppCheckHeader, out StringValues headerValue);
        if (!isHeaderExists)
        {
            _logger.LogError($"no header {FirebaseConstants.FirebaseAppCheckHeader} exists in request");
            await Unauthorized(context);
            return;
        }

        string token = headerValue.ToString();

        if (token == _firebaseSettings.DebugToken)
        {
            _logger.LogInformation("request token {token} is whitelisted as debug token", token);
            await next(context);
            return;
        }

        string jwksAsJson = await GetJwks();
        var handler = new JwtSecurityTokenHandler();
        var jwks = new JsonWebKeySet(jwksAsJson).GetSigningKeys();

        TokenValidationParameters validationParameters = new()
        {
            IssuerSigningKeys = jwks,
            ValidateAudience = true,
            ValidAudience = FirebaseConstants.Audience,
            ValidateIssuer = true,
            ValidIssuer = FirebaseConstants.Issuer,
            ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
        };

        try
        {
            ClaimsPrincipal claims = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is null)
            {
                _logger.LogError("request token {token} is null", token);
                await Unauthorized(context);
                return;
            }

            if (validatedToken!.ValidTo < DateTime.UtcNow)
            {
                _logger.LogError("request token {token} has expired", token);
                await Unauthorized(context);
                return;
            }

            if (!claims.Claims.Any(x => x.Value == FirebaseConstants.Audience))
            {
                _logger.LogError("request token {token} did not contain project id in claims", token);
                await Unauthorized(context);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "could not validate token {token} in request", token);
            await Unauthorized(context);
            return;
        }

        _logger.LogInformation("token {token} validated successfully", token);

        await next(context);
    }

    private async Task<string> GetJwks()
    {
        bool isInCache = _cache.TryGetValue(FirebaseConstants.JwksCacheKey, out string? jwksFromCache);
        if (isInCache)
        {
            return jwksFromCache!;
        }

        string jwksFromApi = await _googleApiClient.GetStringAsync(FirebaseConstants.JwksPath);

        _cache.Set(FirebaseConstants.JwksCacheKey, jwksFromApi, TimeSpan.FromHours(5));

        return jwksFromApi;
    }

    private static async Task Unauthorized(HttpContext context)
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("invalid token");
    }
}
