#! /bin/bash
git pull
set -e
./node_modules/.bin/ng build -prod --aot
cd dist
ln -s assets/img/
ln -s assets/favicon
ln -s assets/favicon.ico
