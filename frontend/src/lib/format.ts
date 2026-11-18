// The API stores and returns UTC; the kiosk shows Thai local time.
// "en-GB" gives dd/MM/yyyy with the Gregorian (C.E.) year as in the mockup;
// "th-TH" would switch to the Buddhist year (e.g. 2564 instead of 2021).
const TIME_ZONE = "Asia/Bangkok";

const dateFormat = new Intl.DateTimeFormat("en-GB", {
  timeZone: TIME_ZONE,
  day: "2-digit",
  month: "2-digit",
  year: "numeric",
});

const timeFormat = new Intl.DateTimeFormat("en-GB", {
  timeZone: TIME_ZONE,
  hour: "2-digit",
  minute: "2-digit",
  hourCycle: "h23",
});

/** "2021-10-15T07:00:00+00:00" → { date: "15/10/2021", time: "14:00" } */
export function formatIssuedAt(isoUtc: string): { date: string; time: string } {
  const instant = new Date(isoUtc);
  return { date: dateFormat.format(instant), time: timeFormat.format(instant) };
}

/** Accepts only a positive whole number that fits in a JS number; anything else is "not found". */
export function parseTicketId(value: string): number | null {
  if (!/^[1-9]\d{0,15}$/.test(value)) {
    return null;
  }
  const id = Number(value);
  return Number.isSafeInteger(id) ? id : null;
}
