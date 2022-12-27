# Database

## Setup test env

```sql
CREATE DATABASE `naheulbook` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_test` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_integration` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE USER 'naheulbook'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook'@'localhost';
GRANT  ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook'@'localhost';
GRANT  ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook'@'localhost';
CREATE USER 'naheulbook_dev'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook_dev'@'localhost';
CREATE USER 'naheulbook_test'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook_test'@'localhost';
CREATE USER 'naheulbook_integration'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook_integration'@'localhost';
FLUSH PRIVILEGES;
```

## Apply migrations

### Install dotnet-fm

* See https://fluentmigrator.github.io/articles/runners/dotnet-fm.html

### Apply migration

* `dotnet fm migrate -p mysql -c "Server=localhost;Database=naheulbook;Uid=naheulbook;Pwd=naheulbook;SslMode=None" -a bin/Debug/Naheulbook.DatabaseMigrator.dll`