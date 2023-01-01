#!/bin/env bash
set -e
mkdir -p backups
mv -f backups/naheulbook.sql.gz backups/naheulbook.sql.gz.1
/usr/bin/mysqldump \
	--routines \
	--triggers \
	-u root \
	--host 172.11.1.2 \
	--password=PUT_SOME_RANDOM_HERE \
	naheulbook | gzip > backups/naheulbook.sql.gz
