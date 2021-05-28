# Общие сведения
Часть подробных сведений о методах можно получить сваггером:
https://billing.evarun.ru/swagger/v1/index.html

Ответ на все методы имеет структуру json:
```
{
  "data": 
  {
    //something
  },
  "status": true,
  "message": "string"
}
```
Где, data - это объект или коллекция объектов, если такие ожидаются.
status - успешность запроса.
message - описание ошибки, если status = false

# Методы требуюущие авторизацию.
Требуется  
Cookie: Authorization=token
или 
Header: Authorization=token

Шаблон URL: https://gateway.evarun.ru/api/v1/billing/{url}
## Админка, требуют наличия у пользователя прав администратора. Выдаются по запросу.
* GET a-users, a-skus, a-sku, a-nomenklaturas, a-nomenklatura, a-specialisations, a-specialisation, a-shops, a-shop, a-corporations, a-corporation, a-producttypes, a-producttype - получение 
* POST a-add-shop, a-add-specialisation, a-add-nomenklatura, a-add-sku - добавление 
* PATCH a-edit-shop, a-edit-specialisation, a-edit-nomenklatura, a-edit-sku - изменение 
* DELETE a-del-shop, a-del-nomenklatura, a-del-sku - удаление 
* POST a-set-specialisation, a-drop-specialisation - установка/сброс специализации магазину

## Приложение.

* GET sin - получение информации по SIN для авторизованного персонажа.
```
  "data": {
    "modelId": ID, 
    "sin": "string",
    "currentBalance": 0,
    "personName": "string",
    "currentScoring": 0,
    "lifeStyle": "string",
    "forecastLifeStyle": "string",
    "metatype": "string",
    "citizenship": "string",
    "nationality": "string",
    "status": "string",
    "nation": "string",
    "viza": "string",
    "pledgee": "string",
    "insurance": "string",
    "licenses": [
      "string"
    ]
  }
```
modelId - общий идентификатор персонажа. 
sin, currentBalance, personName, currentScoring, lifeStyle, forecastLifeStyle - Информационные значения, которые необходимо отображать на экране экономика-обзор. 
nationality, status, nation, viza, pledgee, insurance, licenses - Значения, которые необходимо отображать на экране паспорт.

* GET rentas, - получение списка рент для авторизованного персонажа.
Возвращается ответ:
```
"data": {
    "rentas": [
      {
        "modelId": "string",
        "characterName": "string",
        "rentaId": 0,
        "finalPrice": 0,
        "productType": "string",
        "specialisation": "string",
        "nomenklaturaName": "string",
        "skuName": "string",
        "corporation": "string",
        "shop": "string",
        "hasQRWrite": true,
        "qrRecorded": "string",
        "priceId": 0,
        "dateCreated": "2021-04-14T11:05:00.713Z"
      }
    ],
    "sum": 0
```
rentas - массив рент.
sum - общая сумма по рентам.
finalPrice, dateCreated, skuName, corporation, shop - Поля которые необходимо отображать на экране подробности ренты.

* GET  transfers - получение списка трансферов
Возвращается ответ:
```
 "data": {
    "transfers": [
      {
        "modelId": "string",
        "transferType": "string",
        "newBalance": 0,
        "comment": "string",
        "amount": 0,
        "operationTime": "2021-04-14T11:20:15.600Z",
        "from": "string",
        "to": "string",
        "anonimous": true
      }
    ]
```
from, to, operationTime, newBalance, comment - поля необходимые отображать на вкладке подробности операции.

* GET /api/scoring/info/getmyscoring - получение детализированной информации по скорингу игрока.
Возвращается ответ:
```
 "data": {
    "character": 0,
    "currentFix": 0,
    "currentRelative": 0,
    "relativeCategories": [
      {
        "name": "string",
        "value": 0,
        "weight": 0,
        "factors": [
          {
            "name": "string",
            "value": 0
          }
        ]
      }
    ],
    "fixCategories": [
      {
        "name": "string",
        "value": 0,
        "weight": 0,
        "factors": [
          {
            "name": "string",
            "value": 0
          }
        ]
      }
    ]
  },

```
currentFix, currentRelative - текущие значения составляющие итоговый скоринг.
relativeCategories - категории, вес которых будет изменяться на игре. 
fixCategories - категории, вес которых не будет меняться на игре.
weight - вес категорий.
value - значение для текущего пользователя
factors - факторы составляющие категории

* POST /createtransfersinsin
```
Принимаемая модель:
{
  "characterTo": 0,
  "amount": 0,
  "comment": "string",
  "sinTo": "string"
}
```
sinTo - приоритет на использования этого поля. Если оно не указано, то берется characterTo.
characterTo - ModelId игрока которому будет выполняться перевод, не может быть равен 0 или персонажем отправителем.
amount - размер перевода. Должен быть положительным и меньше текущего баланса.
comment - комментарий, ограничение в 255 символов.

## Сайт магазина.

# Методы, не требующие авторизационный хедер, но доступные через gateway.

# Методы, не требующие авторизационный хедер, доступны только по прямому запросу.
## Генерация персонажа.
* POST api/billing/admin/initcharacter/{modelid} - инициализация персонажа для указанного modelId. Обнуляет скоринг, кошелек, ренты, переводы.
## Карта страховок.
* GET insurance/getinsurance - получение информации по страховке

# Коды ошибок
* 403 - ошибка авторизации. Авторизационный токен присутствует, но у пользователя нет доступа к запрашиваемой странице.
* 404 - объект указанный в параметрах не найден.
* 401 - авторизационный токен отсутствует.
* 422 - ошибка бизнес логики(например не хватает денег на перевод.
* 500 - необрабатываемые ошибки.
