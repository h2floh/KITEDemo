// These outputs can be used for local docker container ENV Variables
output "AzureConnectionString" {
  value = "RunAs=App;AppId=${azuread_application.KITEDemoApp.application_id};TenantId=${data.azurerm_client_config.current.tenant_id};AppKey=${azuread_application_password.KITEDemoApp.value}"
}
output "KeyVaultName" {
  value = var.application_name
}
output "TenantId" {
  value = data.azurerm_client_config.current.tenant_id
}
output "ClientId" {
  value = azuread_application.KITEDemoApp.application_id
}
output "RedirectUri" {
  value = var.reply_url
}
output "Scope" {
  value = "${azuread_application.KITEDemoApp.identifier_uris[0]}/${azuread_application.KITEDemoApp.oauth2_permissions[0].value}"
}
output "CertificateName" {
  value = azurerm_key_vault_certificate.APISSL.name
}
output "IoTHubName" {
  value = azurerm_iothub.KITEHub.name
}