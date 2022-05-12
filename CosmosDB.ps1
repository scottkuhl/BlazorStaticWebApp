$Prog = "C:\Program Files\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe"
$Process = "Microsoft.Azure.Cosmos.Emulator"
$Running = Get-Process $Process -ErrorAction SilentlyContinue

if($Running -eq $null)
{
	& $Prog
}