using System.ServiceProcess;
using WindowsUpdateStopper.Helpers;

namespace WindowsUpdateStopper.MainWorkerService;
public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly IServiceHelper _serviceHelper;
	private readonly List<OsService> ShouldTurnServices = new List<OsService>
	{
		new OsService { Name = "wuauserv", DisplayName = "Windows Update" },
		new OsService { Name = "WaaSMedicSvc", DisplayName = "Windows Update Medic", ShouldChangeStartType = false }//currently, this service can't be changed StartType
	};

	public Worker(ILogger<Worker> logger, IServiceHelper serviceHelper)
	{
		_logger = logger;
		_serviceHelper = serviceHelper;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				foreach (var svc in ShouldTurnServices)
				{
					var process = _serviceHelper.GetService(svc.Name);
					if (process != null && (process.Status != ServiceControllerStatus.Stopped && process.Status != ServiceControllerStatus.StopPending))
					{
						process.Stop();
						_logger.LogInformation($"Found and stopped {svc.DisplayName} service");
					}

					if (!svc.ShouldChangeStartType)
						continue;

					if (process?.StartType != ServiceStartMode.Disabled)
					{
						string oldStartType = process.StartType.ToString();
						_serviceHelper.SetStartMode(process, ServiceStartModeEx.Disabled);
						_logger.LogInformation($"Set {svc.DisplayName} service's mode from {oldStartType} to be {ServiceStartMode.Disabled.ToString()}");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message + Environment.NewLine + ex.InnerException?.Message);
			}

			await Task.Delay(1000, stoppingToken);
		}
	}
}