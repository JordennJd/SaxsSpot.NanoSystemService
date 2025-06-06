FROM liquibase/liquibase:latest

# Копируем changelogs и проперти
COPY ./IncrementalScripts /liquibase/IncrementalScripts
COPY liquibase.properties /liquibase/liquibase.properties
COPY entrypoint.sh /liquibase/entrypoint.sh
COPY config.xml /liquibase/config.xml

USER root
# Разрешаем запуск скрипта
RUN chmod +x /liquibase/entrypoint.sh

# Устанавливаем PostgreSQL JDBC-драйвер, если нужно
ADD https://jdbc.postgresql.org/download/postgresql-42.2.20.jar /liquibase/lib/postgresql-42.2.20.jar

ENTRYPOINT ["/liquibase/entrypoint.sh"]