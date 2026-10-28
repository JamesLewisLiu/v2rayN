namespace SingBox.WebAPI.Security;

public static class WebDeploymentSecurityPolicy
{
    public const string MissingManagementKeyMessage =
        "Management Key is required in supervised/container deployments. Set SING_BOX_WEB_API_KEY before starting sing-box.WebAPI.";

    public static string? GetStartupError(bool supervisedEnvironment, bool containerEnvironment, string? managementKey) =>
        (supervisedEnvironment || containerEnvironment) && string.IsNullOrWhiteSpace(managementKey)
            ? MissingManagementKeyMessage
            : null;
}
