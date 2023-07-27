using Serilog;
using System.IO;
using System.Reflection;
using WindowsUpdateStopper.Helpers;

namespace WindowsUpdateStopper.MainWorkerService;
public class Program
{
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
			.MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
			.Enrich.FromLogContext()
			.WriteTo.File(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constants.LOG_FILENAME), 
							shared: true, 
							outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] - [{Level:u3}]: {Message:lj}{NewLine}{Exception}",
							fileSizeLimitBytes: 10000000,
							rollOnFileSizeLimit: true   //when the file reaches fileSizeLimitBytes, will roll to new file
							)
			.CreateLogger();
		//shared: true  => allow other processes to access this log file concurrently

		//equivalent to appsettings.json
		/*
		{
			"Logging": {
				"Debug": {
					"LogLevel": {
						"Default": "Information",
						"Microsoft": "Warning"
						"System.Net.Http.HttpClient": "Warning"

  }
				}
			}
		}
		*/

		try
		{
			Log.Information("Windows Update Stopper starts running");
			CreateHostBuilder(args).Build().Run();
		}
		catch (Exception ex)
		{
			Log.Fatal(ex, "There was a problem starting the service");
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.UseWindowsService()        //use WorkerService as WindowsService
			.UseSerilog()
			.ConfigureServices((hostContext, services) =>
			{
				services.AddHttpClient();   //register for httpClient
				services.AddHostedService<Worker>();
				services.AddScoped<IServiceHelper, ServiceHelper>();
				services.AddScoped<ICommonHelper, CommonHelper>();
			});
}