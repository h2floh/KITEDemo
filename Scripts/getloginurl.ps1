$TENANTID=$(terraform output -state="..\Infra\terraform.tfstate" TenantId)
$CLIENT_ID=$(terraform output -state="..\Infra\terraform.tfstate" ClientId)
$REDIRECT_URI=[System.Web.HttpUtility]::UrlEncode($(terraform output -state="..\Infra\terraform.tfstate" RedirectUri))
$SCOPE=[System.Web.HttpUtility]::UrlEncode($(terraform output -state="..\Infra\terraform.tfstate" Scope))
Write-Host "Implicit:"
Write-Host "https://login.microsoftonline.com/$TENANTID/oauth2/v2.0/authorize?client_id=$CLIENT_ID&response_type=token&redirect_uri=$REDIRECT_URI&response_mode=fragment&scope=openid%20$SCOPE"
Write-Host ""
Write-Host "Code:"
Write-Host "1."
Write-Host "https://login.microsoftonline.com/$TENANTID/oauth2/v2.0/authorize?client_id=$CLIENT_ID&response_type=code&redirect_uri=$REDIRECT_URI&response_mode=query&scope=openid%20offline_access%20$SCOPE"
Write-Host "2."
Write-Host "https://login.microsoftonline.com/$TENANTID/oauth2/v2.0/token"
Write-Host "client_id=$CLIENT_ID&redirect_uri=$REDIRECT_URI&grant_type=authorization_code&response_mode=query&scope=openid%20offline_access%20$SCOPE&client_secret=&code="

