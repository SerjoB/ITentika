# ITentikaApp

Система генерации и обработки событий
---

## Требования

- .NET 6.0 SDK
- PostgreSQL

---

## Установка и запуск

### Запуск локально с PostgreSQL

Склонируйте репозиторий:

```bash
git clone <URL_репозитория>
cd ITentika\ItentikaApp
```

Настройте строку подключения в appsettings.json в папке проекта для вашего пользователя БД:

```json
{
  "ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=handbookdb;Username=postgres;Password=psql1"
  }
}
```

Убедитесь, что база данных существует и PostgreSQL запущен локально.

Пример запуска программы:

```bash
dotnet run
```

В любом браузере перейти по http://localhost:5000/swagger/index.html
