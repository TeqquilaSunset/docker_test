# Регестрация проверки здоровья сервисов в консул
Invoke-WebRequest -Method Put -InFile "consul_api_requests\health\rabbitHealth.json" -Uri "http://localhost:8500/v1/agent/check/register"

Invoke-WebRequest -Method Put -InFile "consul_api_requests\health\serviceApiHealth.json" -Uri "http://localhost:8500/v1/agent/check/register"

Invoke-WebRequest -Method Put -InFile "consul_api_requests\health\serviceFrontHealth.json" -Uri "http://localhost:8500/v1/agent/check/register"

