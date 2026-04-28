namespace Mavrynt.BuildingBlocks.Application.Behaviors;

/// <summary>
/// Marker interface for requests that are safe to retry.
/// Only implement on idempotent commands and read-only queries.
///
/// Do NOT implement on mutating, non-idempotent commands such as
/// RegisterUser, LoginUser, ChangePassword — retrying these blindly
/// could cause duplicate side effects.
///
/// The resilience behavior respects this marker to apply retry / timeout logic
/// in the future (currently a no-op pass-through; Polly is not yet introduced).
/// </summary>
public interface IResilientRequest
{
    /// <summary>Maximum retry attempts. Default: 0 (no retry).</summary>
    int MaxRetryAttempts => 0;

    /// <summary>Timeout for a single attempt in milliseconds. 0 means no timeout.</summary>
    int TimeoutMs => 0;
}
