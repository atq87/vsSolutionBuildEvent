.nuget\nuget restore vsSolutionBuildEvent_net40.sln 
"C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe" "vsSolutionBuildEvent_net40.sln" /verbosity:detailed  /l:"packages\vsSBE.CI.MSBuild.1.5.0\bin\CI.MSBuild.dll" /m:12 /t:Rebuild /p:Configuration=CI_Release /p:Platform="Any CPU"