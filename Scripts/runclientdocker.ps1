param(
    [Parameter(Mandatory=$True, HelpMessage="DeviceId")]
    [int] $DeviceId
)

az extension add --name azure-iot
az iot hub device-identity create --hub-name $(terraform output -state="..\Infra\terraform.tfstate" IoTHubName) --device-id $DeviceId

docker run -it -e ConnectionString=$(az iot hub device-identity show-connection-string --hub-name $(terraform output -state="..\Infra\terraform.tfstate" IoTHubName) --device-id $DeviceId --output tsv) h2floh/kitedemoclient