[1mdiff --git a/libs-cs.sln b/libs-cs.sln[m
[1mindex e6347a5..43b03cf 100644[m
[1m--- a/libs-cs.sln[m
[1m+++ b/libs-cs.sln[m
[36m@@ -22,7 +22,9 @@[m [mProject("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Collections", "Libs\Collect[m
 EndProject[m
 Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Extensions", "Libs\Extensions\Extensions.csproj", "{C966EB2D-830E-4524-84FB-DA052666AA0D}"[m
 EndProject[m
[31m-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Misc", "Libs\Misc\Misc.csproj", "{3A4F89E6-5005-499B-8F0B-DA8B1C29091B}"[m
[32m+[m[32mProject("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Misc", "Libs\Misc\Misc.csproj", "{3A4F89E6-5005-499B-8F0B-DA8B1C29091B}"[m
[32m+[m[32mEndProject[m
[32m+[m[32mProject("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Libs", "Libs", "{A19994AB-CDC9-4C51-853C-B2707C2D62DA}"[m
 EndProject[m
 Global[m
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution[m
[36m@@ -58,6 +60,13 @@[m [mGlobal[m
 	GlobalSection(SolutionProperties) = preSolution[m
 		HideSolutionNode = FALSE[m
 	EndGlobalSection[m
[32m+[m	[32mGlobalSection(NestedProjects) = preSolution[m
[32m+[m		[32m{81026E9A-E528-49BA-B2CD-3BD8303ED943} = {A19994AB-CDC9-4C51-853C-B2707C2D62DA}[m
[32m+[m		[32m{FAB01896-E2E1-4C10-B738-3A231B063879} = {A19994AB-CDC9-4C51-853C-B2707C2D62DA}[m
[32m+[m		[32m{2D2DB620-F7B9-4079-8D02-AF5F83662AF0} = {A19994AB-CDC9-4C51-853C-B2707C2D62DA}[m
[32m+[m		[32m{C966EB2D-830E-4524-84FB-DA052666AA0D} = {A19994AB-CDC9-4C51-853C-B2707C2D62DA}[m
[32m+[m		[32m{3A4F89E6-5005-499B-8F0B-DA8B1C29091B} = {A19994AB-CDC9-4C51-853C-B2707C2D62DA}[m
[32m+[m	[32mEndGlobalSection[m
 	GlobalSection(ExtensibilityGlobals) = postSolution[m
 		SolutionGuid = {26279412-2CFC-47A5-B072-80EFDD6D547E}[m
 	EndGlobalSection[m
