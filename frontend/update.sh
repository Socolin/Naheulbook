#! /bin/bash
git pull
set -e
npm update
npm run build
