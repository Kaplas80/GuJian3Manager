#load "nuget:?package=PleOps.Cake&version=0.6.1"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
	info.CoverageTarget = 0;
    info.PreviewNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.PreviewNuGetFeedToken = info.GitHubToken;
    info.StableNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.StableNuGetFeedToken = info.GitHubToken;
	
    info.AddApplicationProjects("GuJian3Tool");
    info.AddLibraryProjects("GuJian3Library");
    info.AddTestProjects("GuJian3Tests");
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
