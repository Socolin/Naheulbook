#! /bin/bash
git pull
set -e
./node_modules/.bin/ng build -prod
cd dist
ln -s assets/css/
ln -s assets/js/
ln -s assets/fonts/
ln -s assets/img/
ln -s assets/favicon
ln -s assets/favicon.ico 


