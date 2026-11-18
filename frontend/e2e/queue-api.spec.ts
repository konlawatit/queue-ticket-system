import { expect, test } from "@playwright/test";
import { API_URL } from "./support/environment";
import { QUEUE_SIZE, codesFromStart, currentCode, issueTickets, resetQueue, type TicketJson } from "./support/queue";

// API-level tests against the real API and SQL Server. They prove what the UI
// cannot: many kiosks pressing at the same moment still get different tickets.

test.beforeEach(async ({ request }) => {
  await resetQueue(request);
});

test("100 concurrent requests get 100 different tickets, A0 to J9", async ({ request }) => {
  const responses = await Promise.all(
    Array.from({ length: 100 }, () => request.post(`${API_URL}/api/queue/tickets`)),
  );

  expect(responses.map((r) => r.status())).toEqual(Array(100).fill(201));

  const tickets = (await Promise.all(responses.map((r) => r.json()))) as TicketJson[];
  const codes = tickets.map((t) => t.code);
  expect(new Set(codes).size).toBe(100);
  expect([...codes].sort()).toEqual(codesFromStart(100));
  expect(new Set(tickets.map((t) => t.id)).size).toBe(100);
  expect(await currentCode(request)).toBe("J9");
});

test("A9 is followed by B0", async ({ request }) => {
  const tickets = await issueTickets(request, 11);

  expect(tickets.map((t) => t.code)).toEqual(codesFromStart(11));
  expect(tickets[9].code).toBe("A9");
  expect(tickets[10].code).toBe("B0");
});

test("after Z9 the queue is full: 409 until it is reset, then A0 again", async ({ request }) => {
  const tickets = await issueTickets(request, QUEUE_SIZE);
  expect(tickets.at(-1)?.code).toBe("Z9");

  const full = await request.post(`${API_URL}/api/queue/tickets`);
  expect(full.status()).toBe(409);
  expect(full.headers()["content-type"]).toContain("application/problem+json");
  expect(await full.json()).toMatchObject({ status: 409, title: "Queue is full" });
  expect(await currentCode(request)).toBe("Z9");

  await resetQueue(request);
  const [next] = await issueTickets(request, 1);
  expect(next.code).toBe("A0");
});

test("a ticket can be read back; an unknown id is 404", async ({ request }) => {
  const [issued] = await issueTickets(request, 1);
  expect(issued.issuedAt).toMatch(/\+00:00$/); // stored and returned in UTC

  const found = await request.get(`${API_URL}/api/queue/tickets/${issued.id}`);
  expect(found.status()).toBe(200);
  expect(await found.json()).toEqual(issued);

  const missing = await request.get(`${API_URL}/api/queue/tickets/999999999`);
  expect(missing.status()).toBe(404);
  expect(missing.headers()["content-type"]).toContain("application/problem+json");
});
