namespace Apeiron.Application.Common.Models;

public class FeatureFlags
{
    public const string SectionName = "FeatureFlags";

    public bool EnableCaching { get; set; } = false;
    public bool EnableOpenTelemetry { get; set; } = true;
    public bool EnableAuth { get; set; } = true;
}
