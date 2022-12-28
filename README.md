# Naheulbook

<p align="center">
![Logo](doc/logo.png)
</p>

## Introduction

- Pour les informations non techniques, voir [le site officiel](https://naheulbook.fr/)
- Pour signaler un bug ou faire une demande de fonctionnalités, vous pouvez créer un ticket dans les [*Issues*](https://github.com/Socolin/NaheulbookBackend/issues)


## Development

### Linux (Docker)

With this option, the MySQL database will be running in a docker on a random assigned port. Redis too will run on a
random assigned port.

If the port change (after a restart for example) you can update your config with the
script `./tools/scripts/update-config.sh`

You can put all your local configuration in `src/Naheulbook.Web/appsettings.local.json` this file will not be committed.

1. Setup the frontend and start it (`npm install; npm start`) (it should run on http://localhost:4200)
2. Prerequisites:
    - `nginx`
    - `jq`
    - `docker` your use should be able to use it
    - `dotnet` https://dotnet.microsoft.com/en-us/download/dotnet/7.0
3. Run the setup script: `sudo ./tools/scripts/setup-dev.sh -u ${USER}`
4. You can start the server:
    - In Rider, by executing the configuration `API - Linux`
    - In command line with `dotnet run --project src/Naheulbook.Web --socket=/var/run/naheulbook/api.sock`

### Linux

Install `nginx`, `mysql`, `redis`.

#### Configuration

Add configuration file `src/Naheulbook.Web/appsettings.local.json`, with appropriate value like in the following
example:

```
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=naheulbook;Uid=naheulbook;Pwd=naheulbook;SslMode=None;CharSet=utf8;AllowPublicKeyRetrieval=True;Port=3306",
    "Redis": "localhost:6379,abortConnect=false"
}
```

#### Mysql

```sql
CREATE DATABASE `naheulbook` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_test` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_integration` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE USER 'naheulbook'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook'@'localhost';
GRANT ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook'@'localhost';
GRANT ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook'@'localhost';
CREATE USER 'naheulbook_dev'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook_dev'@'localhost';
CREATE USER 'naheulbook_test'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook_test'@'localhost';
CREATE USER 'naheulbook_integration'@'localhost' IDENTIFIED BY 'naheulbook';
GRANT ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook_integration'@'localhost';
FLUSH PRIVILEGES;
```

#### Nginx

Then you need to configure `nginx`  you can follow the example in `tools/script/conf.nginx`

### Windows

Not done yet, contact me on Discord if you are interested
