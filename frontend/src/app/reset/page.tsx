import type { Metadata } from "next";
import Link from "next/link";
import { connection } from "next/server";
import { KioskScreen } from "@/components/KioskScreen";
import { ResetPanel } from "@/components/ResetPanel";
import { buttonStyles } from "@/components/styles";
import { getCurrentQueue } from "@/lib/queue-api";

export const metadata: Metadata = { title: "IT 05-3 ล้างคิว" };

/** IT 05-3: shows the current code and resets the queue to "00". */
export default async function ResetPage() {
  // Render on every request: the current code must be live, never baked in at build time.
  await connection();
  const current = await getCurrentQueue();

  return (
    <KioskScreen code="IT 05-3">
      <ResetPanel initialCode={current.code} />
      <Link href="/" className={buttonStyles.primary}>
        กลับไปหน้ารับบัตรคิว
      </Link>
    </KioskScreen>
  );
}
