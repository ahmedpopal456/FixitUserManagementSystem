data "azurerm_key_vault" "main" {
    name = "${var.organization_name}-${var.environment_name}-${var.service_abbreviation}-kv"
    resource_group_name = azurerm_resource_group.main.name
}

data "azurerm_key_vault_secret" "b2cclientsecret" {
  name         = "B2CClientSecret"
  sensitive    = true
  key_vault_id = data.azurerm_key_vault.main.id
}

data "azurerm_key_vault_secret" "b2cappid" {
  name         = "B2CAppId"
  key_vault_id = data.azurerm_key_vault.main.id
}

data "azurerm_key_vault_secret" "b2ctenantid" {
  name         = "B2CTenantId"
  key_vault_id = data.azurerm_key_vault.main.id
}

data "azurerm_cosmosdb_account" "main" {
  name                = "${var.organization_name}-${var.environment_name}-common-cosmosdb"
  resource_group_name = "fixit-dev-common"
}