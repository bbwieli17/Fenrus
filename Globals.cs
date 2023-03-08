namespace Fenrus;

/// <summary>
/// Global variables
/// </summary>
public class Globals
{
    /// <summary>
    /// Gets the version number of Fenrus
    /// </summary>
    public const string Version = "1.0.0.0";
    
    /// <summary>
    /// Gets the UID for the guest dashboard, this is a fixed UID that never changes
    /// </summary>
    public static readonly Guid GuestDashbardUid = new Guid("a8b0858a-aa8d-432a-9e32-e85adf1dfb67");
    
    /// <summary>
    /// Gets the magic string to indicate a list option is actually an option group and should be rendered as such
    /// An option group cannot be selected, it group all options below it
    /// </summary>
    public static string LIST_OPTION_GROUP = "###GROUP###";

    /// <summary>
    /// Gets if this app is running inside a docker container
    /// </summary>
    public static readonly bool IsDocker = Environment.GetEnvironmentVariable("Docker") == "1";

    /// <summary>
    /// Gets the default accent color
    /// </summary>
    public const string DefaultAccentColor = "#FF0090";
    
    /// <summary>
    /// Gets the default background color
    /// </summary>
    public const string DefaultBackgroundColor = "#111111";
}