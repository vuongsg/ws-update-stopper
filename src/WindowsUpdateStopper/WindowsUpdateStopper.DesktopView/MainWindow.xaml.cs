using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsUpdateStopper.Helpers;
using WindowsUpdateStopper.MainWorkerService;

namespace WindowsUpdateStopper.DesktopView;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		ServiceCollection serviceCollection = new ServiceCollection();
		serviceCollection.AddScoped<IServiceHelper, ServiceHelper>();   //must register service again
		serviceCollection.AddScoped<ICommonHelper, CommonHelper>();
		serviceCollection.AddWpfBlazorWebView();

#if DEBUG
		serviceCollection.AddBlazorWebViewDeveloperTools();
#endif

		Resources.Add("services", serviceCollection.BuildServiceProvider());
		InitializeComponent();
	}

	private void Window_Closed(object sender, EventArgs e)
	{
		GlobalVars.LogContent.Clear();
		GlobalVars.PreviousPage = null;
	}
}
