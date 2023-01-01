#!/bin/env bash
set -e

TMP=`mktemp`
curl -L -o $TMP https://nightly.link/Socolin/Naheulbook/workflows/frontend/master/frontend.zip
unzip -o $TMP -d www/
