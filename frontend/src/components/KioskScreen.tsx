import type { ReactNode } from "react";

type KioskScreenProps = {
  /** Screen code shown in the green bar, e.g. "IT 05-1". */
  code: string;
  children: ReactNode;
};

/** The framed screen from the mockups: black border, green title bar, centred content. */
export function KioskScreen({ code, children }: KioskScreenProps) {
  return (
    <main className="mx-auto flex min-h-screen w-full max-w-4xl items-center p-4 sm:p-8">
      <section
        aria-label={code}
        className="flex min-h-[32rem] w-full flex-col border-2 border-black bg-white"
      >
        <header className="border-b-2 border-black bg-kiosk-green py-2 text-center text-sm">
          {code}
        </header>
        <div className="flex flex-1 flex-col items-center gap-6 px-6 py-10 text-center">
          {children}
        </div>
      </section>
    </main>
  );
}
