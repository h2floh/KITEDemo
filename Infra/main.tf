# Configure AzureRM provider
provider "azurerm" {
  version = "~>2.00"
  skip_provider_registration = true
  features {}
}

# Configure the Microsoft Azure Active Directory Provider
provider "azuread" {
  version = "~>0.7.0"
}

provider "random" {
  version = "~>2.2"
}

// Current connection/authorization information for Terraform AzureRM provider
data "azurerm_client_config" "current" {}