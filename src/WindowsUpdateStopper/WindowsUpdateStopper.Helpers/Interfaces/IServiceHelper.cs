using System.ServiceProcess;

namespace WindowsUpdateStopper.Helpers;
public interface IServiceHelper
{
	ServiceController GetService(string serviceName);
	void SetStartMode(ServiceController serviceController, ServiceStartModeEx mode);
}
