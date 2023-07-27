using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindowsUpdateStopper.Helpers;

namespace WindowsUpdateStopper.DesktopView.Components;
public partial class AboutPage
{
	[Inject]
	public NavigationManager UriHelper { get; set; }

	public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	protected void BackToIndex()
	{
		GlobalVars.PreviousPage = "about-page";
	}
}
