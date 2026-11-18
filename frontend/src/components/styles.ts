// Shared Tailwind class lists, so buttons and links look the same on every screen.

const base =
  "inline-flex items-center justify-center text-white transition-colors " +
  "focus-visible:outline-4 focus-visible:outline-offset-4 focus-visible:outline-kiosk-blue " +
  "disabled:cursor-wait disabled:opacity-60";

export const buttonStyles = {
  /** The big "รับบัตรคิว" button on IT 05-1. */
  hero: `${base} h-44 w-80 max-w-full rounded-[2.5rem] border border-kiosk-blue-dark bg-kiosk-blue text-6xl font-bold shadow-md hover:bg-kiosk-blue-dark`,
  primary: `${base} min-h-12 w-60 rounded-xl border border-kiosk-blue-dark bg-kiosk-blue px-6 font-semibold hover:bg-kiosk-blue-dark`,
  secondary: `${base} min-h-12 w-60 rounded-xl border border-kiosk-gray-dark bg-kiosk-gray px-6 hover:bg-kiosk-gray-dark`,
} as const;

/** The large ticket code ("A5") on IT 05-2 and IT 05-3. */
export const ticketCodeStyle = "text-[8rem] font-medium leading-none sm:text-[10rem]";
