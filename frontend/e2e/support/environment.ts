// Ports and database used only by the E2E run, so it never touches the
// development servers (API 5080 / web 3000) or the development database.

export const API_PORT = 5180;
export const WEB_PORT = 3100;

export const API_URL = `http://localhost:${API_PORT}`;
export const WEB_URL = `http://localhost:${WEB_PORT}`;

/**
 * Same SQL Server container as development (docker compose up -d db), separate database.
 * The dev-only sa password is committed on purpose; see the note in docker-compose.yml.
 * CI can point elsewhere with E2E_CONNECTION_STRING.
 */
export const E2E_CONNECTION_STRING =
  process.env.E2E_CONNECTION_STRING ??
  "Server=127.0.0.1,1433;Database=QueueSystem_E2E;User Id=sa;Password=LocalDev_Passw0rd!;TrustServerCertificate=True";
