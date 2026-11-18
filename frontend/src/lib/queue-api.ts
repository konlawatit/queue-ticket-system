import "server-only";

// Client for the .NET queue API. Runs only on the Next.js server (Server Components
// and Server Actions), so the browser never sees API_BASE_URL and no CORS is needed.

export type Ticket = {
  id: number;
  code: string;
  /** ISO 8601 in UTC, e.g. "2026-11-18T07:00:00+00:00". */
  issuedAt: string;
};

export type CurrentQueue = {
  /** Last ticket of the current round, or "00" if none has been issued. */
  code: string;
};

export type IssueTicketResult =
  | { kind: "issued"; ticket: Ticket }
  | { kind: "full" };

const REQUEST_TIMEOUT_MS = 10_000;

function apiUrl(path: string): string {
  const baseUrl = process.env.API_BASE_URL;
  if (!baseUrl) {
    throw new Error("API_BASE_URL is not set (see frontend/.env.development).");
  }
  return new URL(path, baseUrl).toString();
}

async function send(path: string, method: "GET" | "POST"): Promise<Response> {
  return fetch(apiUrl(path), {
    method,
    headers: { Accept: "application/json" },
    // Queue data changes all the time: never serve it from a cache.
    cache: "no-store",
    signal: AbortSignal.timeout(REQUEST_TIMEOUT_MS),
  });
}

async function readJson<T>(response: Response, action: string): Promise<T> {
  if (!response.ok) {
    throw new Error(`${action} failed: ${response.status} ${response.statusText}`);
  }
  return (await response.json()) as T;
}

export async function issueTicket(): Promise<IssueTicketResult> {
  const response = await send("/api/queue/tickets", "POST");
  if (response.status === 409) {
    return { kind: "full" };
  }
  return { kind: "issued", ticket: await readJson<Ticket>(response, "Issue ticket") };
}

/** Returns null when no ticket has this id. */
export async function getTicket(id: number): Promise<Ticket | null> {
  const response = await send(`/api/queue/tickets/${id}`, "GET");
  if (response.status === 404) {
    return null;
  }
  return readJson<Ticket>(response, "Get ticket");
}

export async function getCurrentQueue(): Promise<CurrentQueue> {
  return readJson<CurrentQueue>(await send("/api/queue/current", "GET"), "Get current queue");
}

export async function resetQueue(): Promise<CurrentQueue> {
  return readJson<CurrentQueue>(await send("/api/queue/reset", "POST"), "Reset queue");
}
