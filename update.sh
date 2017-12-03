#! /bin/bash
git pull
set -e
npm update
./node_modules/.bin/ng build --prod
