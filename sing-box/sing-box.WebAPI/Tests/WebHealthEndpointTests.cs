using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SingBox.WebAPI.Api;
using SingBox.WebAPI.Services;

namespace SingBox.WebAPI.Tests;

public class WebHealthEndpointTests
{
    [Test]
    public async Task LanHealthResponseContainsOnlyStatusAndDoesNotExposeRuntimeDetails()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.20");

        var response = WebHealthEndpoint.CreateResponse(context, runtime: null!);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        await json.Should().BeEqualTo("{\"status\":\"ok\"}");
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-instance-pid").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-version").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-state").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-process-ids").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-profile-id").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-shutdown-stage").Should().BeFalse();
    }

    [Test]
    public async Task LoopbackHealthProbeRetainsTheFieldsRequiredByLauncherAndUpdater()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Loopback;
        context.Request.Host = new HostString("127.0.0.1", 5080);
        var runtime = new V2rayRuntime(null!, null!, null!, null!, null!);

        _ = WebHealthEndpoint.CreateResponse(context, runtime);

        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-instance-pid").Should().BeTrue();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-version").Should().BeTrue();
        await context.Response.Headers["X-sing-box-WebAPI-core-state"].ToString().Should().BeEqualTo("stopped");
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-process-ids").Should().BeTrue();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-profile-id").Should().BeTrue();
    }

    [Test]
    public async Task LocalReverseProxyDoesNotGetExtendedHealthFields()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Loopback;
        context.Request.Host = new HostString("127.0.0.1", 5080);
        context.Request.Headers["X-Forwarded-For"] = "198.51.100.25";

        var response = WebHealthEndpoint.CreateResponse(context, runtime: null!);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        await json.Should().BeEqualTo("{\"status\":\"ok\"}");
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-instance-pid").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-process-ids").Should().BeFalse();
        await context.Response.Headers.ContainsKey("X-sing-box-WebAPI-core-profile-id").Should().BeFalse();
    }
}
