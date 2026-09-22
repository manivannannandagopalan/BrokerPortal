CREATE TABLE IF NOT EXISTS "DCTUserAdmin" (
    id integer PRIMARY KEY,
    intparentid integer NULL,
    inttype integer NOT NULL,
    name varchar(50) NOT NULL,
    contact varchar(100) NULL,
    phone varchar(50) NULL,
    phoneext varchar(50) NULL,
    address1 varchar(255) NULL,
    address2 varchar(255) NULL,
    city varchar(100) NULL,
    state varchar(5) NULL,
    zip varchar(10) NULL,
    reference varchar(20) NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX IF NOT EXISTS ix_dct_useradmin_name ON "DCTUserAdmin" (name);
CREATE INDEX IF NOT EXISTS ix_dct_useradmin_intparentid ON "DCTUserAdmin" (intparentid);
