namespace WindowsUpdateStopper.Helpers;
public class CommonHelper : ICommonHelper
{
	public List<string> GetlAllLogFiles()
	{
		var prefixName = Path.Combine(Directory.GetCurrentDirectory(), Constants.LOG_FILENAME.Substring(0, Constants.LOG_FILENAME.LastIndexOf(".")));
		var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory())
								  .Where(f => f.StartsWith(prefixName)).OrderByDescending(f => Path.GetFileNameWithoutExtension(f));

		return files.ToList();
	}

	public string GetLatestLogFile()
	{
		var latestFile = GetlAllLogFiles().FirstOrDefault();
		return latestFile;
	}
}
