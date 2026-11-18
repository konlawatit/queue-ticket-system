import { expect, test, type Page } from "@playwright/test";
import { API_URL } from "./support/environment";
import { QUEUE_SIZE, currentCode, issueTickets, resetQueue, thaiDateTimeText, type TicketJson } from "./support/queue";

// Browser tests for the three screens in the brief (IT 05-1, 05-2, 05-3).

test.beforeEach(async ({ request }) => {
  await resetQueue(request);
});

async function takeTicket(page: Page): Promise<void> {
  await page.getByRole("button", { name: "รับบัตรคิว" }).click();
  await expect(page).toHaveURL(/\/ticket\/\d+$/);
}

/**
 * The framed screen labelled with its code (KioskScreen's <section aria-label>).
 * Matching the code as plain text is ambiguous: Next.js's route announcer also
 * reads the page title, which contains the code, after every client navigation.
 */
function screen(page: Page, code: string) {
  return page.getByRole("region", { name: code });
}

test("full flow: take tickets, reset to 00, start again from A0", async ({ page, request }) => {
  // IT 05-1 → IT 05-2
  await page.goto("/");
  await expect(screen(page, "IT 05-1")).toBeVisible();
  await takeTicket(page);
  await expect(screen(page, "IT 05-2")).toBeVisible();
  await expect(page.getByRole("heading", { name: "หมายเลขคิว" })).toBeVisible();
  await expect(page.getByTestId("ticket-code")).toHaveText("A0");

  // The date and time are the ticket's UTC time shown in Thai time.
  const ticketId = page.url().split("/").at(-1);
  const ticket = (await (await request.get(`${API_URL}/api/queue/tickets/${ticketId}`)).json()) as TicketJson;
  await expect(page.getByTestId("issued-at")).toHaveText(thaiDateTimeText(ticket.issuedAt));

  // Back to IT 05-1, take the next ticket.
  await page.getByRole("link", { name: "กลับไปหน้ารับบัตรคิว" }).click();
  await expect(page).toHaveURL("/");
  await takeTicket(page);
  await expect(page.getByTestId("ticket-code")).toHaveText("A1");

  // IT 05-1 → IT 05-3: the current code is the last ticket; reset shows 00.
  await page.getByRole("link", { name: "กลับไปหน้ารับบัตรคิว" }).click();
  await page.getByRole("link", { name: "ล้างคิว" }).click();
  await expect(page).toHaveURL("/reset");
  await expect(screen(page, "IT 05-3")).toBeVisible();
  await expect(page.getByTestId("current-code")).toHaveText("A1");
  await page.getByRole("button", { name: "ล้างคิว" }).click();
  await expect(page.getByTestId("current-code")).toHaveText("00");

  // IT 05-3 → IT 05-1: numbering starts again.
  await page.getByRole("link", { name: "กลับไปหน้ารับบัตรคิว" }).click();
  await takeTicket(page);
  await expect(page.getByTestId("ticket-code")).toHaveText("A0");
});

test("A9 is followed by B0 (example from the brief)", async ({ page, request }) => {
  await issueTickets(request, 10); // A0..A9

  await page.goto("/");
  await takeTicket(page);

  await expect(page.getByTestId("ticket-code")).toHaveText("B0");
});

test("refreshing the ticket page does not issue another ticket", async ({ page, request }) => {
  await page.goto("/");
  await takeTicket(page);
  await expect(page.getByTestId("ticket-code")).toHaveText("A0");

  await page.reload();

  await expect(page.getByTestId("ticket-code")).toHaveText("A0");
  expect(await currentCode(request)).toBe("A0");
});

test("a double click issues only one ticket", async ({ page, request }) => {
  await page.goto("/");

  await page.getByRole("button", { name: "รับบัตรคิว" }).dblclick();
  await expect(page).toHaveURL(/\/ticket\/\d+$/);

  await expect(page.getByTestId("ticket-code")).toHaveText("A0");
  expect(await currentCode(request)).toBe("A0");
});

test("when the queue is full the kiosk says so and stays on IT 05-1", async ({ page, request }) => {
  await issueTickets(request, QUEUE_SIZE); // up to Z9

  await page.goto("/");
  await page.getByRole("button", { name: "รับบัตรคิว" }).click();

  // Filter by text: Next.js's route announcer is also role="alert".
  await expect(page.getByRole("alert").filter({ hasText: "คิวเต็มแล้ว" })).toBeVisible();
  await expect(page).toHaveURL("/");
  await expect(screen(page, "IT 05-1")).toBeVisible();
});

test("an unknown ticket shows a not-found page with status 404", async ({ page }) => {
  const response = await page.goto("/ticket/999999999");

  expect(response?.status()).toBe(404);
  await expect(page.getByRole("heading", { name: "ไม่พบหมายเลขคิวนี้" })).toBeVisible();
});
