#!/bin/sh
set -e

cd "$(dirname "$0")"

case "${1:-}" in
  docker|--d|-d)
    docker compose up -d
    echo "Waiting for db to come alive"
    sleep 10
    echo "Done"
    ;;
esac

if [ ! -d "./Migrations" ]; then
  dotnet ef migrations add initMigration
fi

dotnet run
