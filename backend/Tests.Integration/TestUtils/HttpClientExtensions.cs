using System.Net.Http.Headers;
using Store.Domain.Enums;
using Tests.Integration.Data;
using Tests.Integration.Fakes;

namespace Tests.Integration.TestUtils;

public static class HttpClientExtensions
{
    public static void AuthenticateAsAsync(
        this HttpClient client,
        int userId,
        string userEmail,
        string fullName,
        UserRole role)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemaName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, userEmail);
        client.DefaultRequestHeaders.Add(TestAuthHandler.NameHeader, fullName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role.ToString());
    }
        
}