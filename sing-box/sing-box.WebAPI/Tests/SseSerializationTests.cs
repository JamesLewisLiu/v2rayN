using SingBox.WebAPI.Api;

namespace SingBox.WebAPI.Tests;

public class SseSerializationTests
{
    [Test]
    public async Task NullEventDataSerializesAsJsonNull()
    {
        await WebApiEndpoints.SerializeEventData(null).Should().BeEqualTo("null");
    }
}
