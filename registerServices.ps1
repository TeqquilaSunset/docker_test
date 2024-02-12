# Регестрация сервисов в consul
Invoke-WebRequest -Method Put -InFile "consul_api_requests\services\rabbit.json" -Uri "http://localhost:8500/v1/agent/service/register"

Invoke-WebRequest -Method Put -InFile "consul_api_requests\services\serviceApi.json" -Uri "http://localhost:8500/v1/agent/service/register"

Invoke-WebRequest -Method Put -InFile "consul_api_requests\services\serviceFront.json" -Uri "http://localhost:8500/v1/agent/service/register"

