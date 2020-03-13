// Display Name of AAD Application
variable "application_name" {
    default = "KiteDemoPlatformAPI"
}

// Region for services
variable "region" {
    default = "koreacentral"
}

// Container Image for API Backend
variable "backendimage" {
  default = "h2floh/kitedemoapi"
}

// Key Value for ssl cert
variable "APISSLkey" {
  default = "ssl"
}

// File path for pfx_file
variable "pfx_file" {
  default = "../SSL/sslcert.pfx"
}

// Import Password for PFX file
variable "pfx_password" {
  
}

// Reply URL
variable "reply_url" {
  default = "https://localhost:5001"
}

// Azure Active Directoy Application password
resource "random_password" "aadapppassword" {
  length      = 24
  min_upper   = 1
  min_lower   = 1
  min_numeric = 1
  min_special = 1
  special     = true
}

// Azure Active Directory Application password expiration date
variable "azuread_application_password_end_date" {
  default = "2030-12-31T00:00:00.00Z"
}