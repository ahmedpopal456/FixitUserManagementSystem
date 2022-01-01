variable "subscription_name" {
  type = string
}

variable "subscription_id" {
  type = string
}

variable "environment_name" {
  type = string
}

variable "location_name" {
  type = string
}

variable "google_api_key" {
    type    = string
}

variable "organization_name" {
  type    = string
  default = "fixit"
}

variable "tenant_id" {
  type    = string
  default = "ccc68497-f4c0-4c2c-b499-78c30c54b52c"
}

variable "service_abbreviation" {
  type    = string
  default = "ums"
}

variable "function_apps" {
  type = list(string)
  default = [
    "api",
    "trigger",
  ]
}

variable "cosmosdb_tables" {
  type = map(string)
  default = {
    users = "Users",
    userratings = "UserRatings"
  }
}
