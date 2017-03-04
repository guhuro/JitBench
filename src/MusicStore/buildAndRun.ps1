dotnet restore; 

if ($LastExitCode -ne 0)
{
    throw "dotnet restore failed."
}

dotnet publish -c Release -f netcoreapp10; 

if ($LastExitCode -ne 0)
{
    throw "dotnet publish failed."
}

cd .\bin\Release\netcoreapp1.0\publish\; 

.\Invoke-Crossgen.ps1; &'C:\Program Files\dotnet\dotnet.exe' .\MusicStore.dll; 

if ($LastExitCode -ne 0)
{
    throw "Invoke-Crossgen failed."
}
cd ..\..\..\..  