// Resource Group for services
resource "azurerm_resource_group" "KITEDemoApp" {
  name     = "rg-${var.application_name}"
  location = var.region

}

// Central Configuration Store
resource "azurerm_key_vault" "KITEDemoApp" {
  name                        = var.application_name
  location                    = azurerm_resource_group.KITEDemoApp.location
  resource_group_name         = azurerm_resource_group.KITEDemoApp.name
  enabled_for_disk_encryption = false
  tenant_id                   = data.azurerm_client_config.current.tenant_id

  sku_name = "standard"
  
  network_acls {
    default_action = "Allow"
    bypass = "None"
  }

  # current connection read/write
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
        "get",
        "set",
        "delete",
        "list"
    ]

    certificate_permissions = [
        "get",
        "import",
        "delete",
        "list"
    ]
  }

  # Application read
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = azuread_service_principal.KITEDemoApp.object_id
    #application_id = azuread_application.KITEDemoApp.application_id
    
    secret_permissions = [
        "get",
        "list"
    ]

    certificate_permissions = [
        "get",
        "list"
    ]
  }

  # Backend Container
  access_policy {

    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = azurerm_container_group.BackendAPI.identity[0].principal_id

    secret_permissions = [
        "get",
        "list"
    ]

    certificate_permissions = [
        "get",
        "list"
    ]
  }
}

# Configuration of Key Vault
# "AzureAd": {
#     "TenantId": "",
#     "ClientId": ""
# },

#AzureAdTenantId
resource "azurerm_key_vault_secret" "AzureAdTenantId" {
  name         = "AzureAd--TenantId"
  value        = data.azurerm_client_config.current.tenant_id
  key_vault_id = azurerm_key_vault.KITEDemoApp.id
}

#AzureAdClientId
resource "azurerm_key_vault_secret" "AzureAdClientId" {
  name         = "AzureAd--ClientId"
  value        = azuread_application.KITEDemoApp.application_id
  key_vault_id = azurerm_key_vault.KITEDemoApp.id
}

# #AzureAdRedirectUrl
# resource "azurerm_key_vault_secret" "AzureAdRedirect" {
#   name         = "AzureAd--RedirectUrl"
#   value        = var.reply_url
#   key_vault_id = azurerm_key_vault.KITEDemoApp.id
# }

#APIName of the installation
resource "azurerm_key_vault_secret" "AzureAdAPIName" {
  name         = "APIName"
  value        = var.application_name
  key_vault_id = azurerm_key_vault.KITEDemoApp.id
}

#AzureAdClientSecret (not needed yet)
# resource "azurerm_key_vault_secret" "AzureAdClientSecret" {
#   name         = "AzureAd--ClientSecret"
#   value        = azuread_application_password.KITEDemoApp.value
#   key_vault_id = azurerm_key_vault.KITEDemoApp.id
# }

resource "azurerm_key_vault_certificate" "APISSL" {
  name         = var.APISSLkey
  key_vault_id = azurerm_key_vault.KITEDemoApp.id

  certificate {
    contents = filebase64(var.pfx_file)
    password = var.pfx_password
  }

  certificate_policy {

    issuer_parameters {
      name = "Unknown"
    }

    key_properties {
      exportable = true
      key_size   = 2048
      key_type   = "RSA"
      reuse_key  = false
    }

    secret_properties {
      content_type = "application/x-pkcs12"
    }
  }
}