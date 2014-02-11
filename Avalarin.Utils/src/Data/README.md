Утилиты для работы с БД
========

### Примеры использования:

Вызов хранимой процедуры с несколькими входными параметрами и одним выходным параметром, чтение всех записей:
```csharp
using (var conn = GetConnection()) {
    int totalCount;
    conn.Sp("public.get_users")
        .WithParameters(new { page = 1, pageSize = 25 })
        .WithOutputParameter<int>("totalCount", c => totalCount = c)
        .ExecuteAndReadAll(MapUser);
}
...
private User MapUser(IDataReader reader) {
    return new User() {
        Id = reader.Value<Guid>("id"),
        Name = reader.Value<string>("name"),
        Email = reader.Value<string>("email"),
        RegistrationDate = reader.Value<DateTime>("reg_date"),
        LastLoginDate = reader.ValueOrDefault<DateTime>("login_date")
        ...
    };
}
```

Вызов хранимой процедуры и чтение первой записи:
```csharp
using (var conn = GetConnection()) {
    conn.Sp("public.get_user_by_id")
        .WithParameters(new { id = 5 })
        .ExecuteAndReadFirstOrDefault(MapUser);
}
```


Выполение текстового запроса и получение скалярного значения:
```csharp
using (var conn = GetConnection()) {
    var count = (int)conn.Text("select count(*) from public.user").ExecuteScalar();
}
```

Выполение хранимой процедуры без получения результатов:
```csharp
using (var conn = GetConnection()) {
    conn.Sp("public.remove_user_by_id")
        .WithParameters(new { id = 5 })
        .ExecuteNonQuery();
}
```
