# Definition des ressources Azure

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.43.0"
    }
  }
  required_version = "1.3.8"
}

provider "azurerm" {
  features {}
  tenant_id       = ""
  subscription_id = ""
}

resource "azurerm_resource_group" "gaming_1" {
  name     = var.rg_name
  location = var.rg_location
}

resource "azurerm_service_plan" "gaming_1" {
  name                = var.asp_name
  location            = azurerm_resource_group.gaming_1.location
  resource_group_name = azurerm_resource_group.gaming_1.name
  os_type             = "Windows"
  sku_name            = var.asp_sku_name
}

resource "azurerm_windows_web_app" "gaming_1" {
  name                = var.app_name
  resource_group_name = azurerm_resource_group.gaming_1.name
  location            = azurerm_service_plan.gaming_1.location
  service_plan_id     = azurerm_service_plan.gaming_1.id

  site_config {
    application_stack {
      dotnet_version = var.dotnet_version
    }
  }
}

resource "azurerm_mssql_server" "gaming_1" {
  name                         = var.db_server_name
  resource_group_name          = azurerm_resource_group.gaming_1.name
  location                     = azurerm_resource_group.gaming_1.location
  version                      = "12.0"
  administrator_login          = var.db_login
  administrator_login_password = var.db_password

  tags = {
    environment = "production"
  }
}

resource "azurerm_mssql_database" "gaming_1" {
  name           = var.db_name
  server_id      = azurerm_mssql_server.gaming_1.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 1
  read_scale     = false
  sku_name       = "S0"
  zone_redundant = false
}