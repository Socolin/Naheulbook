#!/usr/bin/env bash
set -e
# Uncomment this for debug
set -x

OPTIND=1
user=""

function show_help {
  echo "$0 OPTION"
  echo "    -u user   Specify the user for which the setup should be done"
}

while getopts "h?u:" opt; do
  case "$opt" in
  h | \?)
    show_help
    exit 0
    ;;
  u)
    user=${OPTARG}
    ;;
  esac
done

if [[ "${user}" = "" ]]; then
  echo 'Missing -u user argument'
  exit 1
fi

if ! which jq >/dev/null; then
  echo 'jq not found'
  exit 1
fi

if ! which nginx >/dev/null; then
  echo 'Nginx not found'
  exit 1
fi

if ! which docker >/dev/null; then
  echo 'Docker not found'
  exit 1
fi

NAHEULBOOK_ROOT_PATH=$(pwd)
while [ ! -f ${NAHEULBOOK_ROOT_PATH}/Naheulbook.slnx ]; do
  NAHEULBOOK_ROOT_PATH=${NAHEULBOOK_ROOT_PATH}/..
  if [ "$(realpath "$NAHEULBOOK_ROOT_PATH")" == '/' ]; then
    echo "File 'Naheulbook.slnx' was not found in the current directory or in a parent"
    exit 1
  fi
done
NAHEULBOOK_ROOT_PATH=$(realpath "${NAHEULBOOK_ROOT_PATH}")
echo "Naheulbook root: ${NAHEULBOOK_ROOT_PATH}"

# Add script to create /var/run/naheulbook at startup
cat >/etc/init.d/naheulbook-dev <<EOF
#!/bin/sh
### BEGIN INIT INFO
# Provides: naheulbook-dev
# Default-Start:  2 3 4 5
# Default-Stop: 0 1 6
### END INIT INFO
mkdir -p /var/run/naheulbook
chown -R "${user}":www-data /var/run/naheulbook
chmod -R g+s /var/run/naheulbook
EOF
chmod +x /etc/init.d/naheulbook-dev
sudo update-rc.d naheulbook-dev defaults
systemctl start naheulbook-dev

# Setup mysql/redis dependencies
sudo -H -u "${user}" -i bash -c "cd ${NAHEULBOOK_ROOT_PATH}/tools/scripts; docker compose up -d"

# Create MySQL users and databases
MYSQL_PORT=$(docker inspect  scripts-naheulbook_dev_env_mysql-1 | jq '.[0].NetworkSettings.Ports."3306/tcp"[0].HostPort | fromjson')

mysql -u root --password=naheulbook --host localhost --protocol=TCP --port "${MYSQL_PORT}"<<-'EOF'
ALTER USER 'root'@'%' IDENTIFIED VIA mysql_native_password USING PASSWORD('naheulbook');
FLUSH PRIVILEGES;
EOF

mysql -u root --password=naheulbook --host localhost --protocol=TCP --port "${MYSQL_PORT}"<<-'EOF'
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
FLUSH PRIVILEGES;
EOF

MYSQL_CONNECTION_STRING="Server=localhost;Database=naheulbook;Uid=naheulbook;Pwd=naheulbook;SslMode=None;CharSet=utf8;AllowPublicKeyRetrieval=True;Port=${MYSQL_PORT}"
sudo -H -u "${user}" -i bash -c "cd ${NAHEULBOOK_ROOT_PATH}; dotnet run --project tools/Naheulbook.DatabaseMigrator.Cli -- --ConnectionStrings:DefaultConnection='${MYSQL_CONNECTION_STRING}' --operation=migrate"
mysql -u root --password=naheulbook --host localhost --protocol=TCP --port "${MYSQL_PORT}" naheulbook  <"${NAHEULBOOK_ROOT_PATH}/src/Naheulbook.DatabaseMigrator/init_data.sql"

# Generate dev certificate
sudo -H -u "${user}" -i bash -c "cd ${NAHEULBOOK_ROOT_PATH}; dotnet run --project  tools/Naheulbook.GenerateDevCertificate"

# Add nginx configuration
cat conf.nginx |
  sed s@__CERT_PATH__@"${NAHEULBOOK_ROOT_PATH}"/tls/Naheulbook.crt@ |
  sed s@__CERT_KEY_PATH__@"${NAHEULBOOK_ROOT_PATH}"/tls/Naheulbook.key@ \
    >/etc/nginx/sites-enabled/naheulbook-dev.conf

if ! nginx -t
then
  echo 'Nginx config:'
  echo '══════════════════════════════════════════════'
  cat -n /etc/nginx/sites-enabled/naheulbook-dev.conf
  echo '══════════════════════════════════════════════'
  echo 'Invalid nginx config, reverting...'
  rm -f /etc/nginx/sites-enabled/naheulbook-dev.conf
  exit 1;
fi


systemctl reload nginx
sleep 0.5
curl -qsS https://local.naheulbook.fr >/dev/null

TEMP_CRONTAB=$(sudo -H -u "${user}" -i bash -c "mktemp")
sudo -H -u "${user}" -i bash -c "cd ${NAHEULBOOK_ROOT_PATH}; crontab -l | grep -v '#nahelbook-dev$' > ${TEMP_CRONTAB}"
echo '@reboot '"${NAHEULBOOK_ROOT_PATH}"'/tools/scripts/update-config.sh #nahelbook-dev' | sudo -H -u "${user}" -i bash -c "cat >> ${TEMP_CRONTAB}"
sudo -H -u "${user}" -i bash -c "crontab - <${TEMP_CRONTAB}"

./update-config.sh

echo 'Done'
