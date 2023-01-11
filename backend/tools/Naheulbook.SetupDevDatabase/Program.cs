// See https://aka.ms/new-console-template for more information

using MySql.Data.MySqlClient;
using Naheulbook.Tools.Shared;

var configResolver = new ConfigResolver();

var mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
mySqlConnectionStringBuilder.Port = (uint)await configResolver.GetMysqlPortAsync();
mySqlConnectionStringBuilder.Server = "localhost";
mySqlConnectionStringBuilder.UserID = "root";
mySqlConnectionStringBuilder.Password= "naheulbook";
mySqlConnectionStringBuilder.SslMode= MySqlSslMode.Disabled;
mySqlConnectionStringBuilder.CharacterSet = "utf8";
mySqlConnectionStringBuilder.AllowPublicKeyRetrieval = true;
await using var connection = new MySqlConnection(mySqlConnectionStringBuilder.ConnectionString);
await connection.OpenAsync();

var mysqlInitScript = /* Language=SQL */ """
DROP DATABASE IF EXISTS `naheulbook`;
DROP DATABASE IF EXISTS `naheulbook_test`;
DROP DATABASE IF EXISTS `naheulbook_integration`;
CREATE DATABASE `naheulbook` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_test` CHARACTER SET UTF8 COLLATE utf8_general_ci;
CREATE DATABASE `naheulbook_integration` CHARACTER SET UTF8 COLLATE utf8_general_ci;
DROP USER IF EXISTS `naheulbook`;
DROP USER IF EXISTS `naheulbook_dev`;
DROP USER IF EXISTS `naheulbook_test`;
DROP USER IF EXISTS `naheulbook_integration`;
CREATE USER 'naheulbook'@'%' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook'@'%';
GRANT  ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook'@'%';
GRANT  ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook'@'%';
CREATE USER 'naheulbook_dev'@'%' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook`.* TO 'naheulbook_dev'@'%';
CREATE USER 'naheulbook_test'@'%' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook_test`.* TO 'naheulbook_test'@'%';
CREATE USER 'naheulbook_integration'@'%' IDENTIFIED BY 'naheulbook';
GRANT  ALL PRIVILEGES ON `naheulbook_integration`.* TO 'naheulbook_integration'@'%';
""";

var command = new MySqlCommand(mysqlInitScript, connection);
await command.ExecuteNonQueryAsync();

