#!/usr/bin/env bash
set -e

NAHEULBOOK_ROOT_PATH=$(dirname "${BASH_SOURCE[0]}")
while [ ! -f "${NAHEULBOOK_ROOT_PATH}"/Naheulbook.slnx ]; do
  NAHEULBOOK_ROOT_PATH=${NAHEULBOOK_ROOT_PATH}/..
  if [ "$(realpath "$NAHEULBOOK_ROOT_PATH")" == '/' ]; then
    echo "File 'Naheulbook.slnx' was not found in the current directory or in a parent"
    exit 1
  fi
done
NAHEULBOOK_ROOT_PATH=$(realpath "${NAHEULBOOK_ROOT_PATH}")
echo "Naheulbook root: ${NAHEULBOOK_ROOT_PATH}"

dotnet run --project ${NAHEULBOOK_ROOT_PATH}/tools/Naheulbook.UpdateDevConfig/Naheulbook.UpdateDevConfig.csproj