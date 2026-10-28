using SingBox.WebAPI.Contracts;

namespace SingBox.WebAPI.Tests;

public class WebApiCapabilitiesTests
{
    [Test]
    public async Task StatusCapabilitiesAreStableUniqueApiIdentifiers()
    {
        var capabilities = WebApiCapabilities.Current;

        await capabilities.Distinct(StringComparer.Ordinal).Count().Should().BeEqualTo(capabilities.Length);
        await capabilities.Should().Contain("events.sse");
        await capabilities.Should().Contain("editor.options");
    }
}
