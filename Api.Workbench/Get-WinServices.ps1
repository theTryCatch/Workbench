[CmdletBinding()]
Param(
	[Parameter(
		Mandatory = $false,
		HelpMessage = "Enter the service name [Not DisplayName]"
	)]$ServiceName,

	[Parameter(
		Mandatory = $false,
		HelpMessage = "Enter the service name [Not DisplayName]"
	)]$ServiceStatus
)
$services = if([string]::IsNullOrEmpty($ServiceName) -eq $false)
{
	Write-Output -InputObject (Get-Service -Name $ServiceName | Select-Object -Property Name, DisplayName, Status)
}
else
{
	Write-Output -InputObject (Get-Service | Select-Object -Property Name, DisplayName, Status)
}

if([string]::IsNullOrEmpty($Status))
{
	return ($services | Select-Object -Property Name, DisplayName, Status)
}
else
{
	return ($services | Where-Object -FilterScript {$_.Status -eq $ServiceStatus} | Select-Object -Property Name, DisplayName, Status)
}