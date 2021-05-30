ARG Version=1.0.0

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
ARG app
ARG Version
COPY . /app/
WORKDIR /app/
RUN dotnet publish "/app/src/${app}/${app}.csproj" -c Release -r alpine-x64 /p:Version=${Version}

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine
ARG app
ARG Version
LABEL "app.owner"="im5tu" "app.repo"="https://github.com/im5tu/aspect"
ENV exe=/app/${app}
WORKDIR /app
COPY --from=build ./app/artifacts/app .
USER aspect
ENTRYPOINT $exe
