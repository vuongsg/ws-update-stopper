using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsUpdateStopper.MainWorkerService;

namespace WindowsUpdateStopper.DesktopView;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private Mutex mutex;

	protected override void OnStartup(StartupEventArgs e)
	{
		bool isNewInstance;
		mutex = new Mutex(true, "WindowsUpdateStopper.DesktopView", out isNewInstance);

		if (!isNewInstance)
		{
			MessageBox.Show("Application is already running.", "", MessageBoxButton.OK, MessageBoxImage.Information);
			App.Current.Shutdown();
		}

		base.OnStartup(e);
	}

	protected override void OnExit(ExitEventArgs e)
	{
		try
		{
			mutex?.ReleaseMutex();
		}
		catch
		{
			//exception: Object synchronization method was called from an unsynchronized block of code
		}

		base.OnExit(e);
	}
}
