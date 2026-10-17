# ---------- Stage 1: build ----------
# Why the SDK image: it has the compiler and NuGet tooling needed to build.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Why: whithout it your files sit in the same folder as the Linux system folders cuz 
# inside a Docker build you’re working in a small Linux filesystem.
# It works, but it’s messy, and your src/ could collide with something system-related
# With WORKDIR /repo: your files are untouched and everything of your src/ is in separate /repo folder
# This folder exists only in the build stage.
WORKDIR /repo

# Why .csproj files first: restore only needs these, so Docker caches this layer
# until a package or project reference changes. Editing a .cs file won't redo it.
# If you have Directory.Build.props, Directory.Packages.props, global.json or
# nuget.config, copy them here too (restore reads them). Add the lines only
# for files that exist, because COPY fails on a missing file.
# COPY Directory.Build.props Directory.Packages.props ./

# One line per project, keeping the folder path, because project references
# need the same relative layout as the repo.ArchTests is left out on purpose:
# it is a test project and the Web project does not reference it.
COPY SchoolPortal.SharedKernel/SchoolPortal.SharedKernel.csproj SchoolPortal.SharedKernel/
COPY SchoolPortal.Students.Domain/SchoolPortal.Students.Domain.csproj SchoolPortal.Students.Domain/
COPY SchoolPortal.Students.Infrastructure/SchoolPortal.Students.Infrastructure.csproj SchoolPortal.Students.Infrastructure/
COPY SchoolPortal.Students.Application/SchoolPortal.Students.Application.csproj SchoolPortal.Students.Application/
COPY SchoolPortal.Grades.Domain/SchoolPortal.Grades.Domain.csproj SchoolPortal.Grades.Domain/
COPY SchoolPortal.Grades.Application/SchoolPortal.Grades.Application.csproj SchoolPortal.Grades.Application/
COPY SchoolPortal.Grades.Infrastructure/SchoolPortal.Grades.Infrastructure.csproj SchoolPortal.Grades.Infrastructure/
COPY SchoolPortal.Attendances.Infrastructure/SchoolPortal.Attendances.Infrastructure.csproj SchoolPortal.Attendances.Infrastructure/
COPY SchoolPortal.Attendances.Domain/SchoolPortal.Attendances.Domain.csproj SchoolPortal.Attendances.Domain/
COPY SchoolPortal.Attendances.Application/SchoolPortal.Attendances.Application.csproj SchoolPortal.Attendances.Application/
COPY SchoolPortal.Identities.Infrastructure/SchoolPortal.Identities.Infrastructure.csproj SchoolPortal.Identities.Infrastructure/
COPY SchoolPortal.Identities.Domain/SchoolPortal.Identities.Domain.csproj SchoolPortal.Identities.Domain/
COPY SchoolPortal.Identities.Application/SchoolPortal.Identities.Application.csproj SchoolPortal.Identities.Application/
COPY SchoolPortal.Notifications.Infrastructure/SchoolPortal.Notifications.Infrastructure.csproj SchoolPortal.Notifications.Infrastructure/
COPY SchoolPortal.Notifications.Domain/SchoolPortal.Notifications.Domain.csproj SchoolPortal.Notifications.Domain/
COPY SchoolPortal.Notifications.Application/SchoolPortal.Notifications.Application.csproj SchoolPortal.Notifications.Application/
COPY SchoolPortal.Email.Infrastructure/SchoolPortal.Email.Infrastructure.csproj SchoolPortal.Email.Infrastructure/
COPY SchoolPortal.Email.Domain/SchoolPortal.Email.Domain.csproj SchoolPortal.Email.Domain/
COPY SchoolPortal.Web/SchoolPortal.Web.csproj SchoolPortal.Web/

# Why: downloads the NuGet packages for Web and every project it references,
# and writes obj/project.assets.json (what the compiler reads). It is the slow,
# internet-bound step, so it gets its own layer that Docker can cache.
RUN dotnet restore SchoolPortal.Web/SchoolPortal.Web.csproj

# Why copy the source only now: any code change invalidates from here down,
# but the slow restore layer above stays cached.
COPY . .

# Why: -c Release turns the compiler optimizer on and removes debug-only code
# (#if DEBUG, Debug.Assert). Default is Debug, which runs slower and is meant
# for stepping through code in an IDE.
# --no-restore: restore already ran in its own cached layer.
# -o /app: one known folder for stage 2 to copy.
RUN dotnet publish SchoolPortal.Web/SchoolPortal.Web.csproj -c Release -o /app --no-restore


# ---------- Stage 2: runtime ----------
# Why aspnet and not sdk: it only has what's needed to run the app.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Why copy before USER: files stay owned by root, so the app can read but not
# modify its own binaries. A compromised app can't rewrite itself.
COPY --from=build /app .

# Why http://+:8080: "+" means all interfaces. Default localhost binding would
# be unreachable from outside the container, so -p 8080:8080 would do nothing.
# Why 8080: a non-root user can't bind ports below 1024.
# http://+:8080 the same as http://0.0.0.0:8080
ENV ASPNETCORE_URLS=http://+:8080

# Why EXPOSE: documentation only. It records "this app listens on 8080" in the image
# metadata for people and tools. It does NOT open or publish anything.
# The -p flag at docker run does that.
EXPOSE 8080

# Why non-root: if the app is exploited, the attacker isn't root in the container.
# APP_UID is defined by the aspnet image for its built-in "app" user.
USER $APP_UID

# Why exec form (JSON array): dotnet runs directly as PID 1, so docker stop's
# SIGTERM reaches the app and it shuts down gracefully. Shell form would put
# /bin/sh in front, and it may not forward the signal.
ENTRYPOINT ["dotnet", "SchoolPortal.Web.dll"]