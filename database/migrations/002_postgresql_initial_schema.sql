CREATE TABLE IF NOT EXISTS users (
    user_id uuid PRIMARY KEY,
    display_name varchar(200) NOT NULL,
    email varchar(320) NOT NULL,
    broker_id uuid NOT NULL,
    role varchar(40) NOT NULL CHECK (role IN ('broker_admin', 'operations', 'viewer')),
    status varchar(20) NOT NULL CHECK (status IN ('Active', 'Pending', 'Suspended')),
    last_sign_in timestamptz NULL,
    CONSTRAINT ux_users_email_broker UNIQUE (email, broker_id)
);