# E-Commerce Analytics Dashboard API

API для сбора и предоставления аналитических данных для e‑commerce дашборда.
---

## Краткое описание

Проект реализует REST API, которое собирает, обрабатывает и отдаёт метрики и отчёты для аналитической панели интернет‑магазина: продажи, конверсии, поведение клиентов, ретеншн и т.д. Backend написан на C# (.NET) и спроектирован для лёгкой интеграции с фронтендом дашборда и системами визуализации.

## Особенности

* Готовые endpoints для метрик: продажи, средний чек, LTV, retention, conversion rate.
* CRUD для сущностей: `products`, `orders`, `customers`. (в будущем)
* Поддержка пагинации и фильтрации по датам/категориям.
* Агрегации по дням/неделям/месяцам.
* Swagger/OpenAPI для быстрой разработки и тестирования.
* Unit и интеграционные тесты (проект `*.Tests`).

## Стек технологий

* C# / .NET 8
* Entity Framework Core
* PostgreSQL
* Swagger
* xUnit / NUnit для тестирования
* Docker (в будущем)

## Требования

* .NET SDK (рекомендуется 8.0)
* Git
* Docker
* Работающая СУБД (Postgres / SQL Server)

## Быстрый старт (локально)

1. Клонировать репозиторий:

```bash
git clone https://github.com/wargen230/E-Commerce-Analytics-Dashboard-API.git
cd E-Commerce-Analytics-Dashboard-API
```

2. Скопировать пример конфигурации и настроить переменные окружения (см. раздел `Переменные окружения`):

```bash
cp .env.example .env
# или установите переменные другим способом
```

3. Восстановить зависимости и запустить проект:

```bash
dotnet restore
dotnet build
dotnet run --project E-Commerce-Analytics-Dashboard-API.API
```

4. Откройте Swagger: `http://localhost:5000/swagger` (или другой порт, который указан в запуске).

> Примечание: если в проекте используется проектное решение (`.sln`), можно запускать из корня через `dotnet run --project ./E-Commerce-Analytics-Dashboard-API.API`.

## Переменные окружения (пример)

Добавьте в `.env` или в конфигурацию приложения:

```env
# Пример для PostgreSQL
CONNECTION_STRING=Host=localhost;Port=5432;Database=ecom_analytics;Username=postgres;Password=pass
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000
LOG_LEVEL=Information
```

## Примеры API (типовые endpoints)

> Ниже — примеры стандартных endpoints. Подкорректируйте пути и параметры под реализацию в коде.

### Получить сводку продаж

`GET /api/metrics/sales?from=2025-01-01&to=2025-01-31&groupBy=day`

Пример:

```bash
curl "http://localhost:5000/api/metrics/sales?from=2025-01-01&to=2025-01-31&groupBy=day"
```

Ответ (пример):

```json
{
  "data": [
    {"date":"2025-01-01","orders":10,"revenue":1250.50},
    {"date":"2025-01-02","orders":15,"revenue":1870.00}
  ],
  "summary": {"totalOrders":25,"totalRevenue":3120.50}
}
```

### Получить список заказов (с фильтрацией/пагинацией)

`GET /api/orders?page=1&pageSize=25&status=completed&from=2025-01-01&to=2025-01-31`

## Аутентификация (в будущем)

## База данных и миграции

Если используется EF Core:

```bash
# создать миграцию
dotnet ef migrations add InitialCreate --project E-Commerce-Analytics-Dashboard-API.API
# применить миграции
dotnet ef database update --project E-Commerce-Analytics-Dashboard-API.API
```

## Docker (в будщем)

## Тесты

Запуск модульных тестов:

```bash
dotnet test ./E-Commerce-Analytics-Dashboard-API.Tests
```

## Contribution

1. Форкните репозиторий
2. Создайте ветку `feature/имя` или `fix/имя`
3. Создайте PR с описанием изменений
