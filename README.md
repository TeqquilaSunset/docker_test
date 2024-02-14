## О приложении
Это небольшое приложение которые выдаёт случайный прогноз на день.

![image](https://github.com/TeqquilaSunset/docker_test/assets/86839875/a34ac50d-d7e9-43b1-8db6-d2be5e268929)

### Запуск
Достаточно набрать в консоле `docker-compose up` и все запустится автоматически.
Также необходимо будет выполнить скрипт `./registerInConsul.ps1` чтобы зарегестрировать сервисы и проверку их здоровья в Consul.


### После Запуска
Доступы к веб интерфейсам можно получить по следующим адрессам:
```
fabio - localhost:9998
consul - localhost:8500
service2_front - localhost:9999/index (через fabio)
rabbit - localhost:15672 (напрямую, не через fabio)
```
