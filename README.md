Часть подробных сведений о методах можно получить сваггером:
https://billing.evarun.ru/swagger/v1/index.html

Ответ на все методы имеет структуру json:
```
{
  "Data": 
  {
    //something
  },
  "Status": true,
  "Message": "string"
}
```
Где, Data - это объект или коллекция объектов, если такие ожидаются.
Status - успешность запроса.
Message - описание ошибки, если status = false

# Методы требуюущие авторизационный хедер(Authorization если обращаться через gateway).
Шаблон URL: https://gateway.evarun.ru/api/v1/billing/{url}
## Админка, требуют наличия у пользователя прав администратора. Выдаются по запросу.
* GET a-users, a-skus, a-sku, a-nomenklaturas, a-nomenklatura, a-specialisations, a-specialisation, a-shops, a-shop, a-corporations, a-corporation, a-producttypes, a-producttype - получение 
* POST a-add-shop, a-add-specialisation, a-add-nomenklatura, a-add-sku - добавление 
* PATCH a-edit-shop, a-edit-specialisation, a-edit-nomenklatura, a-edit-sku - изменение 
* DELETE a-del-shop, a-del-nomenklatura, a-del-sku - удаление 
* POST a-set-specialisation, a-drop-specialisation - установка/сброс специализации магазину

## Приложение.
* GET sin - получение информации по SIN для авторизованного персонажа.
* GET rentas, transfers - получение списка рент/переводов для авторизованного персонажа.



## Сайт магазина.

# Методы, не требующие авторизационный хедер, но доступные через gateway.

# Методы, не требующие авторизационный хедер, доступны только по прямому запросу.
## Генерация персонажа.
* POST api/billing/admin/initcharacter/{modelid} -инициализация персонажа для указанного modelId. Обнуляет скоринг, кошелек.
## Карта страховок.
* GET insurance/getinsurance - получение информации по страховке
