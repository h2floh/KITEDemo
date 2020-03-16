// Backend API as Container Instance
resource "azurerm_container_group" "BackendAPI" {
  name                = "${var.application_name}-aci"
  location            = azurerm_resource_group.KITEDemoApp.location
  resource_group_name = azurerm_resource_group.KITEDemoApp.name
  ip_address_type     = "public"
  dns_name_label      = var.application_name
  os_type             = "Linux"
  restart_policy      = "Never"

  container {
    name   = "backendapi"
    image  = var.backendimage
    cpu    = "0.5"
    memory = "1"

    ports {
      port     = 443
      protocol = "TCP"
    }

    environment_variables = {
      KeyVaultName    = var.application_name
      CertificateName = var.APISSLkey
    }

    secure_environment_variables = {
        AzureConnectionString = "RunAs=App;AppId=${azuread_application.KITEDemoApp.application_id};TenantId=${data.azurerm_client_config.current.tenant_id};AppKey=${azuread_application_password.KITEDemoApp.value}"
    }
  }

  identity {
    type = "SystemAssigned"
  }
}

// IoT Hub
resource "azurerm_iothub" "KITEHub" {
  name                = "${var.application_name}-iothub"
  location            = azurerm_resource_group.KITEDemoApp.location
  resource_group_name = azurerm_resource_group.KITEDemoApp.name

  sku {
    name     = "F1"
    capacity = "1"
  }
}

// IoT Hub Shared Access Policy
resource "azurerm_iothub_shared_access_policy" "KITEBackend" {
  name                = "KITEBackend"
  resource_group_name = azurerm_resource_group.KITEDemoApp.name
  iothub_name         = azurerm_iothub.KITEHub.name

  service_connect = true
}

