Методы API можно вызвать с помощью POST http://instance.evarun.ru:7009/api/{ApiManager}/{MethodName}/
Описание моделей позже.
Все методы возвращают ответ формата:
ResultStatus Status - статус ответа. 
string Message - текст ошибки, если была ошибка
T Data - объект, если метод должен возвращать объект

# ApiManagers
- [Job](#JobManager)
- [Billing](#BillingManager)
- [Shop](#ShopManager)
- [Admin](#AdminManager)
- [Work](#WorkManager)
- [Scoring](#ScoringManager)

## <a name="JobManager"></a>Job(регулярные джобы)
- BaseJob ScheduleJob(JobType type, DateTime startTime, DateTime endTime, string cron) - создать новую джобу, джоба создастся в состоянии stopped.
- bool StartJob(string id) - запустить созданную ранее регулярную джобу
- bool StopJob(string id) - прервать регулярную джобу
- bool DeleteJob(string id) - удалить регулярную джобу
- List<BaseJob> GetAllJobs() - получить список всех джоб

## <a name="BillingManager"></a>Billing(переводы)
- BaseInformation GetBaseInformation(int sin) - получение базовой информации: сумма на счете, сумма регулярных платежей, категорию лайфстайла, уровень коэффициента скоринга, коэфициент ИКАР; 
- Transfer CreateTransfer(string walletFrom, string walletTo, decimal amount, string comment) - прямой перевод между двумя кошельками; 
- Transfer CreateTransferSINSIN(int sinFrom, int sinTo, decimal amount, string comment) - перевод между двумя физическими лицами; 
- Transfer CreateTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment) -  перевод с физлица на юрлицо; 
- Transfer CreateTransferLegSIN(int sinFrom, int legTo, decimal amount, string comment) - перевод от юрлица физическому лицу; 
- Transfer CreateTransferLegLeg(int legFrom, int legTo, decimal amount, string comment) - перевод между двумя юрлицами; 
- List<Price> GetPrices(int sinId, int shopId) - получить список предложений магазина для sin; 
- Price GetPrice(int sinId, int skuId) - получить предложение на указанный продукт для sin; 
- Renta CreateRenta(int priceId) - создать договор ренты; 

## <a name="ShopManager"></a>Shop(Магазин)
- Product CreateProduct(int prototypeId, int corporationId, string name, int count) - создать продукт для корпы; 
- SKU CreateSKU(int productId, int shopId, int price, int count) - создать SKU для магазина; 
- TODO

## <a name="AdminManager"></a>Admin(Админка)
- Bool CreateSIN(SIN sin) - заносит в систему SIN-персонажа и создает связанные с ним сущности(скоринг, кошелек, коэфициенты и т.д.); 
- Bool CreateLegal(Legal legal) - заносит в систему юридическое лицо и создает связанные с ним сущности(кошелек и т.д.); 
- Bool CreateShop(Shop shop) - тоже что и создать юридическое лицо, плюс немного больше; 
- Bool CreateCorporation(Corporation corporation) - тоже что и создать юридическое лицо, плюс немного больше; 
- Bool CreateProductType(ProductType productType) - создать тип товара; 
- Bool CreateGroup(Group group) - создать группу для группировки типов товаров; 
- Bool CreateProductTypeGroup(int prototypeId, int groupId) - создать привязку типа товара к группе; 
- Bool CreateShopGroup(int shopId, int groupId) - создать привязку магазина к группе(этот магазин сможет видеть товары этой группы); 

## <a name="WorkManager"></a>Work(Работочасы и IKAR)
- RateInformation GetRates(int corporationId) - получить список ставок на оплату человекочасов для указанной корпорации; 
- Rate SetRate(int corporationId, int rateType, decimal value) - установить ставку для корпорации на определенную характеристику; 
- Work Hire(int sin, int corporationId) - нанять персонажа на работу в указанную корпорацию; 
- Bool SetIKAR(int value) - установить стоимость одной акции ИКАР; 
- Bool SetIKARToCharacter(int sin, int value) - установить количество акций ИКАР персонажу; 

## <a name="ScoringManager"></a>Scoring
TODO
