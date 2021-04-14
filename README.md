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

# Методы требуюущие авторизацию.
Требуется  
Cookie: Authorization=token

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
  "Data": {
    "ModelId": ID, 
    "SIN": "string",
    "CurrentBalance": 0,
    "PersonName": "string",
    "CurrentScoring": 0,
    "LifeStyle": "string",
    "ForecastLifeStyle": "string",
    "Metatype": "string",
    "Citizenship": "string",
    "Nationality": "string",
    "Status": "string",
    "Nation": "string",
    "Viza": "string",
    "Pledgee": "string",
    "Insurance": "string",
    "Licenses": [
      "string"
    ]
  }
```
ModelId - общий идентификатор персонажа. 
SIN, CurrentBalance, PersonName, CurrentScoring, LifeStyle, ForecastLifeStyle - Информационные значения, которые необходимо отображать на экране экономика-обзор. 
Nationality, Status, Nation, Viza, Pledgee, Insurance, Licenses - Значения, которые необходимо отображать на экране паспорт.
* GET rentas, - получение списка рент для авторизованного персонажа.
```
"Data": {
    "Rentas": [
      {
        "ModelId": "string",
        "CharacterName": "string",
        "RentaId": 0,
        "FinalPrice": 0,
        "ProductType": "string",
        "Specialisation": "string",
        "NomenklaturaName": "string",
        "SkuName": "string",
        "Corporation": "string",
        "Shop": "string",
        "HasQRWrite": true,
        "QRRecorded": "string",
        "PriceId": 0,
        "DateCreated": "2021-04-14T11:05:00.713Z"
      }
    ],
    "Sum": 0
```
Rentas - массив рент
Sum - общая сумма по рентам.
FinalPrice, DateCreated, SkuName, Corporation, Shop - Поля которые необходимо отображать на экране подробности ренты.
* GET  transfers - получение списка трансферов
```
 "Data": {
    "Transfers": [
      {
        "ModelId": "string",
        "TransferType": "string",
        "NewBalance": 0,
        "Comment": "string",
        "Amount": 0,
        "OperationTime": "2021-04-14T11:20:15.600Z",
        "From": "string",
        "To": "string",
        "Anonimous": true
      }
    ]
```
From, To, OperationTime, NewBalance, Comment - поля необходимые отображать на вкладке подробности операции.
* GET /api/Scoring/info/getmyscoring
```
 "Data": {
    "Character": 0,
    "CurrentFix": 0,
    "CurrentRelative": 0,
    "RelativeCategories": [
      {
        "Name": "string",
        "Value": 0,
        "Weight": 0,
        "Factors": [
          {
            "Name": "string",
            "Value": 0
          }
        ]
      }
    ],
    "FixCategories": [
      {
        "Name": "string",
        "Value": 0,
        "Weight": 0,
        "Factors": [
          {
            "Name": "string",
            "Value": 0
          }
        ]
      }
    ]
  },

```
CurrentFix, CurrentRelative - текущие значения составляющие итоговый скоринг.
RelativeCategories - категории, вес которых будет изменяться на игре. 
FixCategories - категории, вес которых не будет меняться на игре.
Weight - вес категорий.
Value - значение для текущего пользователя
Factors - факторы составляющие категории


## Сайт магазина.

# Методы, не требующие авторизационный хедер, но доступные через gateway.

# Методы, не требующие авторизационный хедер, доступны только по прямому запросу.
## Генерация персонажа.
* POST api/billing/admin/initcharacter/{modelid} -инициализация персонажа для указанного modelId. Обнуляет скоринг, кошелек.
## Карта страховок.
* GET insurance/getinsurance - получение информации по страховке
