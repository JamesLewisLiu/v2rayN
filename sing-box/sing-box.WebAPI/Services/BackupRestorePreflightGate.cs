using SingBox.WebAPI.Contracts;
using SingBox.WebAPI.Launcher;

namespace SingBox.WebAPI.Services;

internal static class BackupRestorePreflightGate
{
    public static Task<OperationView> ExecuteAsync(
        NativeWebRestorePreflightResult preflight,
        Func<Task<OperationView>> performRestore)
    {
        if (!preflight.Allowed)
        {
            return Task.FromResult(preflight.ToFailureOperation());
        }

        if (preflight.Owner == WebLifecycleOwner.NativeProcess && preflight.Replacement is null)
        {
            throw new InvalidOperationException("A native restore cannot proceed without a verified restart plan.");
        }

        return performRestore();
    }
}
