// The AAD Application required by Bot Framework Service
resource "azuread_application" "KITEDemoApp" {
    name                       = var.application_name
    identifier_uris            = ["api://${var.application_name}"]
    reply_urls                 = ["msauth://com.companyname.androidapp/DWxBsg%2FQ8zSqNAzwnqv6YIZbJr4%3D", "https://localhost:5001/", "https://localhost:5001/swagger/oauth2-redirect.html", var.reply_url, "${var.reply_url}/swagger/oauth2-redirect.html", "https://global.consent.azure-apim.net/redirect"]
    available_to_other_tenants = true
    oauth2_allow_implicit_flow = true
    type                       = "webapp/api"

    // https://management.azure.com/user_impersonation
    required_resource_access {
      resource_app_id = "797f4846-ba00-4fd7-ba43-dac1f8f63013"

      resource_access {
        id   = "41094075-9dad-400e-a0bd-54e686782033"
        type = "Scope"
      }
    }
    
    # app_role {
    #     allowed_member_types = [
    #         "User",
    #         "Applications"
    #     ]

    #     description  = "Send Commands to Vehicles"
    #     display_name = "Send Commands to Vehicles"
    #     is_enabled   = true
    #     value        = "Control"
    # }
}

resource "azuread_service_principal" "KITEDemoApp" {
  application_id               = azuread_application.KITEDemoApp.application_id
  app_role_assignment_required = false
}

// Adding a password to the Application
resource "azuread_application_password" "KITEDemoApp" {
  application_object_id = azuread_application.KITEDemoApp.object_id
  value                 = random_password.aadapppassword.result
  end_date              = var.azuread_application_password_end_date

  lifecycle {
    ignore_changes = [
      # Ignore changes to end_date
      end_date
    ]
  }
}