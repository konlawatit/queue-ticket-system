import { expect, type APIRequestContext } from "@playwright/test";
import { API_URL } from "./environment";

export type TicketJson = { id: number; code: string; issuedAt: string };

export const QUEUE_SIZE = 260; // A0..Z9

/** Codes in issue order, written out independently of the app: 0 → A0, 10 → B0, 259 → Z9. */
export function codesFromStart(count: number): string[] {
  return Array.from({ length: count }, (_, i) => `${String.fromCharCode(65 + Math.floor(i / 10))}${i % 10}`);
}

export async function resetQueue(request: APIRequestContext): Promise<void> {
  const response = await request.post(`${API_URL}/api/queue/reset`);
  expect(response.status()).toBe(200);
  expect(await response.json()).toEqual({ code: "00" });
}

/** Issues tickets one after another through the API (test setup, not under test). */
export async function issueTickets(request: APIRequestContext, count: number): Promise<TicketJson[]> {
  const tickets: TicketJson[] = [];
  for (let i = 0; i < count; i++) {
    const response = await request.post(`${API_URL}/api/queue/tickets`);
    expect(response.status()).toBe(201);
    tickets.push((await response.json()) as TicketJson);
  }
  return tickets;
}

export async function currentCode(request: APIRequestContext): Promise<string> {
  const response = await request.get(`${API_URL}/api/queue/current`);
  expect(response.status()).toBe(200);
  return ((await response.json()) as { code: string }).code;
}

/** The kiosk shows Thai time: "วันที่ : 15/10/2021 เวลา 14:00 น." */
export function thaiDateTimeText(isoUtc: string): string {
  const instant = new Date(isoUtc);
  const part = (options: Intl.DateTimeFormatOptions) =>
    new Intl.DateTimeFormat("en-GB", { timeZone: "Asia/Bangkok", ...options }).format(instant);
  const date = part({ day: "2-digit", month: "2-digit", year: "numeric" });
  const time = part({ hour: "2-digit", minute: "2-digit", hourCycle: "h23" });
  return `วันที่ : ${date} เวลา ${time} น.`;
}
