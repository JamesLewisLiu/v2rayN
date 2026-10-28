using System.Globalization;
using SingBox.WebAPI.Security;
using SingBox.WebAPI.Services;

namespace SingBox.WebAPI.Api;

internal sealed record WebHealthResponse(string Status);

internal static class WebHealthEndpoint
{
    public static WebHealthResponse CreateResponse(HttpContext context, V2rayRuntime runtime)
    {
        // Native launcher/updater probes connect directly to a loopback address. Require
        // the same peer/Host pair the probe rule accepts, and no forwarding headers, so
        // a local reverse proxy cannot relay internal process diagnostics to a remote
        // caller.
        if (WebSetupAccessPolicy.IsLocalDiagnosticSource(
                context.Connection.RemoteIpAddress, context.Request.Host.Host)
            && WebCorsPolicy.IsSameOriginRequest(context.Request)
            && !WebSetupAccessPolicy.HasForwardedHeaders(context.Request.Headers))
        {
            context.Response.Headers["X-sing-box-WebAPI-instance-pid"] = Environment.ProcessId.ToString(CultureInfo.InvariantCulture);
            context.Response.Headers["X-sing-box-WebAPI-version"] = WebBuildIdentity.Current.Version;
            context.Response.Headers["X-sing-box-WebAPI-core-state"] = runtime.GetCoreRuntimeState();
            context.Response.Headers["X-sing-box-WebAPI-core-process-ids"] = string.Join(',', runtime.GetCoreProcessIds());
            context.Response.Headers["X-sing-box-WebAPI-core-profile-id"] = runtime.GetCoreRuntimeProfileId() ?? string.Empty;
            if (ShutdownDiagnostics.CurrentStage is { Length: > 0 } stage)
            {
                context.Response.Headers["X-sing-box-WebAPI-shutdown-stage"] = stage;
            }
        }

        return new WebHealthResponse("ok");
    }
}
