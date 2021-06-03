ARG Version=1.0.0
ARG app=Aspect

FROM mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-20H2 AS build
ARG app
ARG Version
COPY . /app/
WORKDIR /app/
RUN dotnet publish "/app/src/${app}/${app}.csproj" -c Release -r win-x64 /p:Version=${Version} /p:PublishCli=true

FROM mcr.microsoft.com/windows:20H2
ARG app
ARG Version
LABEL "app.owner"="im5tu" "app.repo"="https://github.com/im5tu/aspect"
ENV exe=/app/${app}
WORKDIR /app
COPY --from=build ./app/artifacts/app .
USER aspect
ENTRYPOINT $exe
