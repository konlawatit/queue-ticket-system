"use client";

import Link from "next/link";
import { useEffect } from "react";
import { KioskScreen } from "@/components/KioskScreen";
import { buttonStyles } from "@/components/styles";

/** Shown when a screen cannot load its data, e.g. the API is down. No technical details reach the user. */
export default function ErrorPage({ error, reset }: { error: Error & { digest?: string }; reset: () => void }) {
  useEffect(() => {
    console.error(error);
  }, [error]);

  return (
    <KioskScreen code="ระบบขัดข้อง">
      <h1 className="text-2xl font-semibold">ไม่สามารถเชื่อมต่อระบบคิวได้</h1>
      <p className="text-lg">กรุณาลองใหม่อีกครั้ง</p>
      <button type="button" onClick={reset} className={buttonStyles.primary}>
        ลองใหม่
      </button>
      <Link href="/" className={buttonStyles.secondary}>
        กลับไปหน้ารับบัตรคิว
      </Link>
    </KioskScreen>
  );
}
