revoke all on schema public from public;
revoke all on all tables in schema public from public;

CREATE DATABASE demo WITH OWNER = postgres;
REVOKE CONNECT ON DATABASE demo FROM PUBLIC;

\connect demo

/*create migrator user, this user own the schema and can modify it */
CREATE ROLE demo_migrator WITH LOGIN PASSWORD 'demo_migrator123' NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION VALID UNTIL 'infinity';
GRANT CONNECT ON DATABASE demo TO demo_migrator;
CREATE SCHEMA service AUTHORIZATION demo_migrator;
ALTER ROLE demo_migrator SET search_path TO service;
GRANT ALL PRIVILEGES ON SCHEMA service TO demo_migrator;


/*create read write user, this user has full access to the data (DML), but can't add tables etc (DDL)*/
CREATE ROLE demo_readwrite WITH LOGIN PASSWORD 'demo_readwrite123' NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION VALID UNTIL 'infinity';
GRANT CONNECT ON DATABASE demo TO demo_readwrite;
ALTER ROLE demo_readwrite SET search_path TO service;
GRANT USAGE on SCHEMA service to demo_readwrite;

ALTER DEFAULT PRIVILEGES for user demo_migrator IN SCHEMA service GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO demo_readwrite;
ALTER DEFAULT PRIVILEGES for user demo_migrator IN SCHEMA service GRANT USAGE, SELECT ON SEQUENCES TO demo_readwrite;
ALTER DEFAULT PRIVILEGES for user demo_migrator IN SCHEMA service GRANT EXECUTE ON FUNCTIONS TO demo_readwrite;

/*create read only user, this user has select access only access to the data*/
CREATE ROLE demo_readonly WITH LOGIN PASSWORD 'demo_readonly123' NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION VALID UNTIL 'infinity';
GRANT CONNECT ON DATABASE demo TO demo_readonly;
GRANT USAGE on SCHEMA service to demo_readonly;
ALTER ROLE demo_readonly SET search_path TO service;

ALTER DEFAULT PRIVILEGES for user demo_migrator IN SCHEMA service GRANT SELECT ON TABLES TO demo_readonly;
ALTER USER demo_readonly set default_transaction_read_only = on;
