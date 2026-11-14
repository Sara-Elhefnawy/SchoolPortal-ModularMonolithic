FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /repo

COPY global.json ./

COPY --parents SchoolPortal.*/*.csproj ./

RUN dotnet restore SchoolPortal.Web/SchoolPortal.Web.csproj

COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

ARG ConnectionStrings__SchoolPortalDb="Host=build-time-placeholder;Database=placeholder;Username=placeholder;Password=placeholder"

RUN set -e; \
    for pair in \
      Students:StudentsDbContext \
      Notifications:NotificationsDbContext \
      Grades:GradesDbContext \
      Identities:IdentityDbContext \
      Attendances:AttendancesDbContext; do \
      m="${pair%%:*}"; ctx="${pair##*:}"; \
      lower=$(echo "$m" | tr '[:upper:]' '[:lower:]'); \
      echo "=== Bundling $m ($ctx) ==="; \
      dotnet restore SchoolPortal.Web/SchoolPortal.Web.csproj; \
      dotnet ef migrations bundle --context "$ctx" \
        --project "SchoolPortal.${m}.Infrastructure/SchoolPortal.${m}.Infrastructure.csproj" \
        --startup-project SchoolPortal.Web/SchoolPortal.Web.csproj \
        -o "/app/bundles/migrate-${lower}" --self-contained -r linux-x64; \
    done

RUN dotnet publish SchoolPortal.Web/SchoolPortal.Web.csproj -c Release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app .

RUN mkdir /keys && chown $APP_UID /keys

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*

USER $APP_UID

ENTRYPOINT ["dotnet", "SchoolPortal.Web.dll"]