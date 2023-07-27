using System.Text;

namespace WindowsUpdateStopper.Helpers;
public class GlobalVars
{
	public static StringBuilder LogContent { get; set; } = new StringBuilder();
	public static string PreviousPage { get; set; }
}
