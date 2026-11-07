# Deployment decision

Last updated: 2026-11-07

## Decision

The SchoolPortal image runs on **SnapDeploy** (free tier). SnapDeploy pulls
`ghcr.io/sara-elhefnawy/schoolportal` from GitHub Container Registry, and the app
connects to a PostgreSQL database that stays on **MonsterASP**.

## How an update reaches production

1. A pull request is merged into `master`.
2. The CI workflow builds, tests, and publishes the image to GHCR with two
   tags: `sha-<short commit>` (one exact build, used for rollbacks) and
   `latest` (the newest build).
3. After I approve the deployment on the `production` GitHub environment, the
   `deploy` job asks SnapDeploy to restart the container.

The GHCR package is **public**, because the free tier runs public images. This
is safe because the image contains no secrets. All settings (connection string,
seed passwords, email password) are provided as environment variables on
SnapDeploy and are never committed to the repository.

## Deploy job

- The `deploy` job runs only on a push to `master`, and only after
  `build-and-push` has published the image.
- It uses the `production` environment, so it pauses until I approve it and
  can't read the environment's secrets before that.
- The SnapDeploy API token is stored as the environment secret
  `SNAPDEPLOY_API_KEY`. The container ID is stored as the environment variable
  `SNAPDEPLOY_CONTAINER_ID`. Only the names are written here, never the values.
- I have not yet verified the job end to end, and in particular I don't know
  whether a restart picks up a new image.

## What the app needs from a host

- Run a Docker image pulled from a registry.
- PostgreSQL (one database, one schema per module).
- A folder for ASP.NET Core Data Protection keys (`/keys`), which protect the
  login cookie and CSRF tokens.
- Outbound email (SMTP).
- No credit card. I can't use platforms that require one, including a
  temporary hold.

## Options considered

- **Render (free):** the filesystem is ephemeral, the free Postgres database
  expires after 30 days, and outbound SMTP ports are blocked. It also asked for
  a card when I tried it earlier.
- **Koyeb (free):** reported to require a card hold, and its free Postgres is
  only a few hours.
- **Azure Container Apps and Google Cloud Run:** both require a card at
  sign-up.
- **Northflank (free sandbox):** sources disagree on whether it needs a card,
  and I did not test it.
- **Caasify:** no card, but the free offer ends on December 31, 2026.
- **Own small VM (Phase 8):** full control, but it takes more setup, so I'm
  keeping it as the next step.

## Why SnapDeploy

- Sign-up needed no credit card.
- It runs a public Docker image, which fits the GHCR package.
- My database already exists on MonsterASP, so I don't need a free database
  from the host (SnapDeploy databases are a paid add-on).
- The free tier does not expire, according to SnapDeploy's pricing page.

## Known limits

- The container sleeps after 15 minutes without traffic and takes about a
  minute to wake up.
- The free tier allows 100 container-hours a month and 10 deploys a day, with
  512 MB RAM and 0.25 vCPU.
- `/keys` is not persistent, because files written inside a free container are
  gone after a sleep or a redeploy. Data Protection keys are lost when the
  container restarts, so users have to log in again after the app wakes up. A
  later improvement is to store the keys in the database.
- The database is on another provider, so every query crosses the internet and
  is slower than a database next to the app.
- Free containers can reach an external database only on ports up to 27017.
  I still need to confirm which port my MonsterASP database uses.
- Email sending from SnapDeploy has not been tested yet.
- Each restart from the deploy job uses one of the 10 free deploys a day.
- The container runs the `latest` tag. The `sha-` tags stay in GHCR, so I can
  roll back by hand to an exact build.
- The SnapDeploy token has the `deploy` scope, and SnapDeploy refuses deletes
  for API tokens, so a leaked token can't delete anything.

## What would change this decision

SnapDeploy offers no documented way to switch a container to a new image tag
automatically. If I need automatic deploys, always-on hosting, persistent
storage, or a database next to the app, I will move to a small VM (Phase 8)
and run the app and PostgreSQL with Docker Compose.