// The AAD Application required by Bot Framework Service
resource "azuread_application" "KITEDemoApp" {
    name                       = var.application_name
    identifier_uris            = ["api://${var.application_name}"]
    reply_urls                 = [var.reply_url, "${var.reply_url}/swagger/oauth2-redirect.html"]
    available_to_other_tenants = false
    oauth2_allow_implicit_flow = true
    type                       = "webapp/api"

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