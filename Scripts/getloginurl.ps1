$TENANTID=$(terraform output -state="..\Infra\terraform.tfstate" TenantId)
$CLIENT_ID=$(terraform output -state="..\Infra\terraform.tfstate" ClientId)
$REDIRECT_URI=[System.Web.HttpUtility]::UrlEncode($(terraform output -state="..\Infra\terraform.tfstate" RedirectUri))
$SCOPE=[System.Web.HttpUtility]::UrlEncode($(terraform output -state="..\Infra\terraform.tfstate" Scope))
return "https://login.microsoftonline.com/$TENANTID/oauth2/v2.0/authorize?client_id=$CLIENT_ID&response_type=token&redirect_uri=$REDIRECT_URI&response_mode=fragment&scope=openid%20$SCOPE"
