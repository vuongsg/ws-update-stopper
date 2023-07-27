using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsUpdateStopper.Helpers;

namespace WindowsUpdateStopper.DesktopView.Components;
public partial class Index
{
	const string sc = @"sc.exe";
	const string displayName = "\"Windows Update Stopper\"";
	string path = Path.Combine(Directory.GetCurrentDirectory(), $"{Constants.SERVICE_NAME}.MainWorkerService.exe");

	[Inject]
	public NavigationManager _uriHelper { get; set; }

	[Inject]
	public IJSRuntime _jsRuntime { get; set; }

	[Inject]
	public IServiceHelper _serviceHelper { get; set; }

	[Inject]
	public ICommonHelper _commonHelper { get; set; }

	protected override Task OnInitializedAsync()
	{
		if (string.IsNullOrEmpty(GlobalVars.PreviousPage))
			CheckStatusStopperService();
		else
			GlobalVars.PreviousPage = null; //intend to keep log state
		
		return base.OnInitializedAsync();
	}

	protected override Task OnAfterRenderAsync(bool firstRender)
	{
		_jsRuntime.InvokeVoidAsync("RestoreLogIdScroll");   //we can only do it in method OnAfterRenderAsync
		return base.OnAfterRenderAsync(firstRender);
	}

	/// <summary>
	/// btn Install's click event
	/// </summary>
	protected void InstallService()
	{
		if (_serviceHelper.GetService(Constants.SERVICE_NAME) != null)
		{
			Uninstall();
		}

		string startService = $"start {Constants.SERVICE_NAME}";

		ProcessStartInfo psi = new ProcessStartInfo("Process.exe")
		{
			FileName = sc,
			Arguments = $"create {Constants.SERVICE_NAME} binpath= \"{path}\" displayName= {displayName}",
			CreateNoWindow = true,
			UseShellExecute = true,
			Verb = "runas"  //run as administrator
		};

		try
		{
			//GlobalVars.LogContent.AppendLine(InfoLog(psi.Arguments));
			Process.Start(psi);
			psi.Arguments = startService;
			//GlobalVars.LogContent.AppendLine(InfoLog(psi.Arguments));
			Process.Start(psi);

			//Thread.Sleep(100);
			////Start again to make sure it will be started automatically
			//ServiceController service = GetService(serviceName);

			//if (service != null && ((service.Status.Equals(ServiceControllerStatus.Stopped)) || (service.Status.Equals(ServiceControllerStatus.StopPending))))
			//{
			//	service.Start();
			//}

			ClearLogDisplay();
			GlobalVars.LogContent.AppendLine(InfoLog($"<p>Install Service successful</p>"));
		}
		catch (Exception ex)
		{
			GlobalVars.LogContent.AppendLine(ErrorLog($"{ex.Message} - {ex.StackTrace} - {ex.Source}"));
		}
	}

	/// <summary>
	/// btn Uninstall's click event
	/// </summary>
	protected void UninstallService()
	{
		Uninstall();
	}

	private void Uninstall()
	{
		string stopService = $"stop {Constants.SERVICE_NAME}";
		string deleteService = $"delete {Constants.SERVICE_NAME}";

		ProcessStartInfo psi = new ProcessStartInfo()
		{
			FileName = sc,
			Arguments = stopService,
			CreateNoWindow = true,
			UseShellExecute = true,
			Verb = "runas"  //run as administrator
		};

		try
		{
			//GlobalVars.LogContent.AppendLine(InfoLog(psi.Arguments));
			Process.Start(psi);
			psi.Arguments = deleteService;
			//GlobalVars.LogContent.AppendLine(InfoLog(psi.Arguments));
			Process.Start(psi);

			ClearLogDisplay();
			GlobalVars.LogContent.AppendLine(InfoLog("Uninstall Service successful"));
		}
		catch (Exception ex)
		{
			GlobalVars.LogContent.AppendLine(ErrorLog(ex.Message));
		}
	}

	/// <summary>
	/// btn Refresh Log's click event
	/// </summary>
	protected void RefreshLog()
	{
		CheckStatusStopperService();
	}

	/// <summary>
	/// btn Load Log's click event
	/// </summary>
	protected void LoadLog()
	{
		_jsRuntime.InvokeVoidAsync("SetLogIdScroll", 0);
		_jsRuntime.InvokeVoidAsync("RestoreLogIdScroll");

		try
		{
			var latestFile = _commonHelper.GetLatestLogFile();
			if (string.IsNullOrEmpty(latestFile))
				latestFile = Path.Combine(Directory.GetCurrentDirectory(), Constants.LOG_FILENAME);

			using (StreamReader sr = new StreamReader(latestFile, new FileStreamOptions { Share = FileShare.ReadWrite }))
			{
				ClearLogDisplay();
				string line;

				while ((line = sr.ReadLine()) != null)
				{
					if (line.Contains("[ERR]"))
						GlobalVars.LogContent.AppendLine(ErrorLog(line));
					else
						GlobalVars.LogContent.AppendLine(InfoLog(line));
				}
			}
		}
		catch (FileNotFoundException ex)
		{
			//await _jsRuntime.InvokeVoidAsync("alert", ex.Message, string.Empty);
			MessageBox.Show(ex.Message, Constants.MAIN_TITLE, MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}

	protected async void ClearOldLogs()
	{
		if (MessageBox.Show("This action will clear all old log files. Please confirm that you want to delete all old log files",
							Constants.MAIN_TITLE,
							MessageBoxButton.YesNo,
							MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			bool hasError = false;

			await (Task.Run(() =>
			{
				var files = _commonHelper.GetlAllLogFiles();
				for (int i = 1, n = files.Count; i < n; i++)
				{
					try
					{
						File.Delete(files[i]);
					}
					catch (Exception ex)
					{
						hasError = true;
						GlobalVars.LogContent.AppendLine(ErrorLog(ex.Message));
					}
				}
			}));

			MessageBox.Show(!hasError ? "Clear all old log files successful" : "Some errors occurred", 
							Constants.MAIN_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

	/// <summary>
	/// btn About's click event
	/// </summary>
	protected void GoToAboutPage()
	{
		GoToOtherPage("about-page");
	}

	protected void GoToOtherPage(string pageName)
	{
		_jsRuntime.InvokeVoidAsync("SetLogIdScroll");
		_uriHelper.NavigateTo(pageName);
	}

	/// <summary>
	/// Check WindowsUpdateStopper service whether it is installed and running or not
	/// </summary>
	private void CheckStatusStopperService()
	{
		ClearLogDisplay();
		var wndUpdateStopper = _serviceHelper.GetService(Constants.SERVICE_NAME);

		if (wndUpdateStopper == null)
			GlobalVars.LogContent.AppendLine(ErrorLog("Please install..."));
		else if (wndUpdateStopper?.Status != System.ServiceProcess.ServiceControllerStatus.Running)
			GlobalVars.LogContent.AppendLine(ErrorLog("Urgent !! Windows Update Stopper is stopped. Please start service immediately !!"));
		else
			GlobalVars.LogContent.AppendLine(InfoLog($"<p><img src=\"images/valid.png\" style=\"width: 20%;\" />  Windows Update Stopper is running</p>"));
	}

	private void ClearLogDisplay()
	{
		GlobalVars.LogContent.Clear();
	}

	private string InfoLog(string message)
	{
		return $"<p class='info-log'>{message}</p>";
	}

	private string ErrorLog(string message)
	{
		return $"<p class='error-log'>{message}</p>";
	}
}
