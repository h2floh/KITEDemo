
$clientId=$(terraform output -state="..\Infra\terraform.tfstate" ClientId) 
$tenant=$(terraform output -state="..\Infra\terraform.tfstate" TenantId) 
$scope=$(terraform output -state="..\Infra\terraform.tfstate" Scope) 
$apiUrl=$(terraform output -state="..\Infra\terraform.tfstate" RedirectUri) + "/Vehicle"

$content="{ 
    ""apiUrl"": ""$apiUrl"",
    ""tenant"": ""$tenant"",
    ""clientId"": ""$clientId"",
    ""scopes"": [
      ""openid"",
      ""offline_access"",
      ""$scope""
    ]
}"

New-Item "..\AndroidApp\settings.json" -ItemType File -Force
Set-Content "..\AndroidApp\settings.json" -Value $content
