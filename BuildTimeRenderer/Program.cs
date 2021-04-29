using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Toolbelt.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Start server...");
        using var serverProcess = XProcess.Start(
            "dotnet", "run --project Server --urls=http://127.0.0.1:5010",
            configure: option => { option.TerminateWhenDisposing = true; });

        var serverStarted = false;
        await foreach (var line in serverProcess.GetOutputAsyncStream())
        {
            Console.WriteLine(line);
            if (line.Trim().StartsWith("Now listening on:"))
            {
                serverStarted = true;
                break;
            }
        }
        if (serverStarted == false) return;

        Console.WriteLine("Start fetching...");

        var httpClient = new HttpClient();
        var htmlContent = await httpClient.GetStringAsync("http://127.0.0.1:5010/");

        var targetDir = Path.Combine("public", "wwwroot");
        Directory.CreateDirectory(targetDir);
        File.WriteAllText(Path.Combine(targetDir, "root.html"), htmlContent);

        Console.WriteLine("Complete.");
    }
}
