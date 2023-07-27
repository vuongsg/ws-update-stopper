namespace WindowsUpdateStopper.Helpers;
public class Constants
{
	public const string SERVICE_NAME = "WindowsUpdateStopper";
	public const string LOG_FILENAME = "log-monitor.txt";
	public const string MAIN_TITLE = "Windows Update Stopper";
}

public enum ServiceStartModeEx
{
	Automatic = 2,
	Manual = 3,
	Disabled = 4,
	DelayedAutomatic = 99
}
