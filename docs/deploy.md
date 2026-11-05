# Deployment decision

Last updated: 2026-11-05

## Decision

The SchoolPortal image runs on **SnapDeploy** (free tier). SnapDeploy pulls
`ghcr.io/yourname/schoolportal` from GitHub Container Registry, and the app
connects to a PostgreSQL database that stays on **MonsterASP**.

## How an update reaches production

1. A pull request is merged into `master`.
2. The CI workflow builds, tests, and publishes the image to GHCR with two
   tags: `sha-<short commit>` (one exact build, used for rollbacks) and
   `latest` (the newest build).
3. SnapDeploy runs the image. [Fill in after the first deploy: how a new
   image is redeployed, automatically or manually.]

The GHCR package is **public**, because the free tier runs public images. This
is safe because the image contains no secrets. All settings (connection string,
seed passwords, email password) are provided as environment variables on
SnapDeploy and are never committed to the repository.

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
  512 MB RAM and 0.25 vCPU. [Confirm these on the dashboard.]
- `/keys` is not persistent [confirm], so Data Protection keys are lost when
  the container restarts. Users have to log in again after the app wakes up.
  A later improvement is to store the keys in the database.
- The database is on another provider, so every query crosses the internet and
  is slower than a database next to the app.
- Email sending from SnapDeploy is not verified yet. [Update after testing.]

## What would change this decision

If I need always-on hosting, persistent storage, or a database next to the app,
I will move to a small VM (Phase 8) and run the app and PostgreSQL with Docker
Compose.