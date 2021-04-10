create table recipes
(
    id        serial       not null
        constraint recipes_pk
            primary key,
    rating    integer,
    imagepath varchar(100),
    name      varchar(100) not null
);

alter table recipes
    owner to postgres;

create unique index recipes_id_uindex
    on recipes using btree (id);

create unique index recipes_name_uindex
    on recipes using btree (name);

create table usedoccasions
(
    id       serial    not null
        constraint usedoccasions_pk
            primary key,
    date     timestamp not null,
    comment  text,
    recipeid integer   not null
        constraint usedoccasions_recipes_id_fk
            references recipes
            on update cascade on delete cascade,
    duration interval  not null
);

alter table usedoccasions
    owner to postgres;

create unique index usedoccasions_id_uindex
    on usedoccasions using btree (id);

create table steps
(
    id          serial       not null
        constraint steps_pk
            primary key,
    number      integer      not null,
    instruction varchar(500) not null,
    recipeid    integer      not null
        constraint steps_recipes_id_fk
            references recipes
            on update cascade on delete cascade
);

alter table steps
    owner to postgres;

create unique index steps_id_uindex
    on steps using btree (id);

create table ingredients
(
    id       serial       not null
        constraint ingredients_pk
            primary key,
    name     varchar(100) not null,
    recipeid integer      not null
        constraint ingredients_recipes_id_fk
            references recipes
            on update cascade on delete cascade
);

alter table ingredients
    owner to postgres;

create unique index ingredients_id_uindex
    on ingredients using btree (id);

create table units
(
    id    integer          not null
        constraint units_pk
            primary key
        constraint units_ingredients_id_fk
            references ingredients
            on update cascade on delete cascade,
    value double precision not null
);

alter table units
    owner to postgres;

create unique index units_id_uindex
    on units using btree (id);

create table volumes
(
    id integer not null
        constraint volumes_pk
            primary key
        constraint volumes_units_id_fk
            references units
            on update cascade on delete cascade
);

alter table volumes
    owner to postgres;

create unique index volumes_id_uindex
    on volumes using btree (id);

create table masses
(
    id integer not null
        constraint masses_pk
            primary key
        constraint masses_units_id_fk
            references units
            on update cascade on delete cascade
);

alter table masses
    owner to postgres;

create unique index masses_id_uindex
    on masses using btree (id);

create table timesteps
(
    id       integer  not null
        constraint timesteps_pk
            primary key
        constraint timesteps_steps_id_fk
            references steps
            on update cascade on delete cascade,
    duration interval not null
);

alter table timesteps
    owner to postgres;

create unique index timesteps_id_uindex
    on timesteps using btree (id);

