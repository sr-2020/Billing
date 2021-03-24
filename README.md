Для получения подробных сведений о методах можно пользоваться сваггером:
https://billing.evarun.ru/swagger/v1/index.html

Ответ на все методы имеет структуру:
{
  "Data": {
    
  },
  "Status": true,
  "Message": "string"
}
Где, Data - это объект или коллекция объектов, если такие ожидаются.
Status - успешность запроса.
Message - описание ошибки, если status = false

# Методы, доступные извне: https://gateway.evarun.ru/api/v1/billing/{url}
## Методы требуюущие авторизационный хедер(Authorization если обращаться через gateway).
### Админка
* GET a-users, a-skus, a-sku, a-nomenklaturas, a-nomenklatura, a-specialisations, a-specialisation, a-shops, a-shop, a-corporations, a-corporation, a-producttypes, a-producttype - получение 

