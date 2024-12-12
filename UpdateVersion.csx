////////////////////////////////////////////////////////////
//
// This is META-file for using in CI system.
//                                         STAM, EpicMorg
////////////////////////////////////////////////////////////

#r "System.Xml"

using System;
using System.IO;
using System.Xml;

var versionFilePath = "kasthack.binding.wf.csproj";
var buildNumber = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "0";

if (buildNumber == "0") {
    Console.WriteLine("GITHUB_RUN_NUMBER environment variable is not set. Defaulting to 0.");
}

var xmlDoc = new XmlDocument();
xmlDoc.Load(versionFilePath);

UpdateVersionNode(xmlDoc, "//Version", buildNumber);
UpdateVersionNode(xmlDoc, "//AssemblyVersion", buildNumber);

xmlDoc.Save(versionFilePath);
Console.WriteLine($"Updated file: {versionFilePath}");

void UpdateVersionNode(XmlDocument xmlDoc, string xpath, string buildNumber) {
    var versionNode = xmlDoc.SelectSingleNode(xpath);

    if (versionNode == null) {
        Console.WriteLine($"Node {xpath} not found.");
        return;
    }
    if (!Version.TryParse(versionNode.InnerText, out var parsedVersion)) {
        Console.WriteLine($"Invalid version format in node {xpath}: {versionNode.InnerText}");
        return;
    }

    var newVersion = new Version(parsedVersion.Major, parsedVersion.Minor, parsedVersion.Build, int.Parse(buildNumber));

    versionNode.InnerText = newVersion.ToString();
    Console.WriteLine($"Updated {xpath} to {newVersion}");
}