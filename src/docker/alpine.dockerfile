FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine
ARG Version=0.0.1
ARG app=Aspect
LABEL "app.owner"="im5tu" "app.repo"="https://github.com/im5tu/aspect"
ENV exe=/app/${app}
WORKDIR /app
COPY ["./artifacts/app/linux-musl-x64", "/usr/bin"]
RUN chmod +x /usr/bin/aspect