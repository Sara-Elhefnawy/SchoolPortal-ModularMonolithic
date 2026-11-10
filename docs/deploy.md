# Deployment decision

Last updated: 2026-11-09

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
   `deploy` job asks SnapDeploy to restart the container, or to start it if it
   is stopped.
4. The job then checks `/health/ready` and fails if the app does not answer
   with HTTP 200.

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
  `SNAPDEPLOY_CONTAINER_ID`, and the public address of the app (used by the
  health check) as the environment variable `APP_BASE_URL`. Only the names are
  written here, never the values.
- The job first asks SnapDeploy to restart the container. Free containers sleep
  after 15 minutes without traffic, and SnapDeploy refuses to restart a
  stopped container (HTTP 409), so in that case the job starts it instead.
  Any other error fails the job.
- After that, the job calls `/health/ready` every 10 seconds, up to 20 times,
  and fails unless it gets HTTP 200. This shows that the app is up, not that
  the new version is the one running. It does not roll anything back
  automatically.
- I have checked that the job reaches SnapDeploy. I have not yet verified that
  a restart or start picks up a new image.

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
  is slower than a database next to the app. The app does connect to it from
  SnapDeploy, which also shows the port is allowed (free containers can reach
  an external database only on ports up to 27017).
- SnapDeploy can reach Gmail's SMTP server. Sending needs a real Gmail app
  password in `Email__AppPassword`, and a wrong password gets an
  authentication error from Gmail.
- Each restart or start from the deploy job uses one of the 10 free deploys a
  day.
- The container runs the `latest` tag. The `sha-` tags stay in GHCR, so I can
  roll back by hand to an exact build.
- A failed health check only turns the run red. Rolling back is manual (see
  below).
- The SnapDeploy token has the `deploy` scope, and SnapDeploy refuses deletes
  for API tokens, so a leaked token can't delete anything.

## Manual rollback

Every merge to `master` publishes an image tagged `sha-<first 7 characters of
the commit>`. These tags never change, so rolling back means running the
previous good tag instead of the bad one.

1. Find the last good version: GitHub, then Packages, then `schoolportal`, then
   the tags list. Or open the Actions run of the last good merge. The tag is
   `sha-` plus the first 7 characters of that commit.
2. In the SnapDeploy dashboard, open the container and change the image tag
   from `latest` to `sha-<previous>`, then redeploy. If the dashboard has no
   such option, create a new container from
   `ghcr.io/sara-elhefnawy/schoolportal:sha-<previous>` with the same port and
   the same environment variables, and start it.
3. Check `/health/ready` on the app's address and the container logs, then try
   the main pages.
4. After the bug is fixed and a new image is merged, set the container back to
   `latest` (or to the new `sha-` tag). While it is pinned to an old tag, the
   deploy job does not change what runs.

Limits of a rollback:

- Rolling back the image does not roll back the database. If the bad release
  included a migration, the old code may not match the new schema. I avoid
  destructive migrations, and a backup is needed to undo one.
- A new container gets a new URL. In that case I update `App__PublicBaseUrl`
  in SnapDeploy, and `SNAPDEPLOY_CONTAINER_ID` and `APP_BASE_URL` in the GitHub
  `production` environment, if the pipeline should point at it.
- Free accounts allow 4 containers and 10 deploys a day.

## What would change this decision

SnapDeploy offers no documented way to switch a container to a new image tag
automatically. If I need automatic deploys, always-on hosting, persistent
storage, or a database next to the app, I will move to a small VM (Phase 8)
and run the app and PostgreSQL with Docker Compose.