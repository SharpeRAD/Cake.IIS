//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Zip-Files")
    .IsDependentOn("Create-NuGet-Packages")
    .IsDependentOn("Publish-Local");

Task("Package-Without-Tests")
    .IsDependentOn("Build")
    .IsDependentOn("Zip-Files")
    .IsDependentOn("Create-NuGet-Packages")
    .IsDependentOn("Publish-Local");

Task("Publish")
	.IsDependentOn("Package")
    .IsDependentOn("Publish-Nuget");

Task("Publish-Without-Tests")
    .IsDependentOn("Package-Without-Tests")
    .IsDependentOn("Publish-Nuget");

Task("AppVeyor")
    .IsDependentOn("Publish")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Slack");
	
Task("Skip-Test")
    .IsDependentOn("Build");

Task("Skip-Restore")
    .IsDependentOn("Build");

Task("Default")
    .IsDependentOn("Publish");
