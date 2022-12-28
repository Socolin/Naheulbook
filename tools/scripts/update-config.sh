#!/usr/bin/env bash
set -e

NAHEULBOOK_ROOT_PATH=$(pwd)
while [ ! -f ${NAHEULBOOK_ROOT_PATH}/NaheulbookV2.sln ]; do
  NAHEULBOOK_ROOT_PATH=${NAHEULBOOK_ROOT_PATH}/..
  if [ "$(realpath "$NAHEULBOOK_ROOT_PATH")" == '/' ]; then
    echo "File 'NaheulbookV2.sln' was not found in the current directory or in a parent"
    exit 1
  fi
done
NAHEULBOOK_ROOT_PATH=$(realpath "${NAHEULBOOK_ROOT_PATH}")
echo "Naheulbook root: ${NAHEULBOOK_ROOT_PATH}"

MYSQL_PORT=$(docker inspect scripts-naheulbook_dev_env_mysql-1 | jq '.[0].NetworkSettings.Ports."3306/tcp"[0].HostPort | fromjson')
CONFIG_PATH="${NAHEULBOOK_ROOT_PATH}/src/Naheulbook.Web/appsettings.local.json"
if ! [ -e "${CONFIG_PATH}" ]; then
  echo '{"ConnectionStrings":{}}' >"${CONFIG_PATH}"
fi

REDIS_PORT=$(docker inspect scripts-naheulbook_dev_env_redis-1 | jq '.[0].NetworkSettings.Ports."6379/tcp"[0].HostPort | fromjson')
REDIS_CONNECTION_STRING="localhost:${REDIS_PORT},abortConnect=false"

TEMP_FILE=$(mktemp)
BACKUP_FILE=$(mktemp)
MYSQL_CONNECTION_STRING="Server=localhost;Database=naheulbook;Uid=naheulbook;Pwd=naheulbook;SslMode=None;CharSet=utf8;AllowPublicKeyRetrieval=True;Port=${MYSQL_PORT}"
cat "${CONFIG_PATH}" |
  jq '.ConnectionStrings.DefaultConnection = "'"${MYSQL_CONNECTION_STRING}"'"' |
  jq '.ConnectionStrings.Redis = "'"${REDIS_CONNECTION_STRING}"'"' >"${TEMP_FILE}"
cp "${CONFIG_PATH}" "${BACKUP_FILE}"
echo "Backup: ${BACKUP_FILE}"
cp "${TEMP_FILE}" "${CONFIG_PATH}"
