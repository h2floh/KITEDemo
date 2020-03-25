#pwsh "C:\repos\KITEDemo\Scripts\kitevehicleteststartup.ps1" -DeviceId 0 -TerraformStateFile "C:\repos\KITEDemo\Infra\terraform.tfstate"
param(
    [Parameter(Mandatory=$True, HelpMessage="DeviceId")]
    [int] $DeviceId,

    [Parameter(Mandatory=$True, HelpMessage="TerraformStateFile")]
    [string] $TerraformStateFile
)

# Write-Host $TerraformStateFile
# Write-Host "terraform output -state='$TerraformStateFile' IoTHubName"
docker run --name "vehicle$DeviceId" -it -e ConnectionString=$(az iot hub device-identity show-connection-string --hub-name $(terraform output -state="$TerraformStateFile" IoTHubName) --device-id $DeviceId --output tsv) h2floh/kitedemoclient
