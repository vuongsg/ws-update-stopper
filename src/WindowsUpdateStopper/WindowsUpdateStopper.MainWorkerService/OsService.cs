namespace WindowsUpdateStopper.MainWorkerService;
public class OsService
{
	public string Name { get; set; }
	public string DisplayName { get; set; }
	public bool ShouldChangeStartType { get; set; } = true;
}
