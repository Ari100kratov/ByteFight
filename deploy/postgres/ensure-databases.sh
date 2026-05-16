#!/usr/bin/env bash
set -euo pipefail

: "${POSTGRES_HOST:=postgres}"
: "${POSTGRES_PORT:=5432}"
: "${POSTGRES_DB:?POSTGRES_DB is required}"
: "${POSTGRES_USER:?POSTGRES_USER is required}"
: "${POSTGRES_CHRONICLES_DB:=bytefight_chronicles}"

if [ "$POSTGRES_CHRONICLES_DB" = "$POSTGRES_DB" ]; then
  echo "Chronicles uses the main PostgreSQL database '$POSTGRES_DB'."
  exit 0
fi

psql \
  --host "$POSTGRES_HOST" \
  --port "$POSTGRES_PORT" \
  --username "$POSTGRES_USER" \
  --dbname postgres \
  --set chronicles_database="$POSTGRES_CHRONICLES_DB" <<'SQL'
SELECT 'CREATE DATABASE ' || quote_ident(:'chronicles_database')
WHERE NOT EXISTS (
    SELECT FROM pg_database WHERE datname = :'chronicles_database'
)\gexec
SQL

echo "PostgreSQL database '$POSTGRES_CHRONICLES_DB' is ready."
