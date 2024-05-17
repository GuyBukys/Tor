namespace Tor.Infrastructure.Authentication;

public static class FirebaseConstants
{
    public const string GoogleApiClient = "googleApi";
    public const string Issuer = "https://firebaseappcheck.googleapis.com/970272774503";
    public const string Audience = "projects/970272774503";
    public const string FirebaseAppCheckHeader = "X-Firebase-AppCheck";
    public const string JwksPath = "/v1beta/jwks";
    public const string JwksCacheKey = "jwks";
}
