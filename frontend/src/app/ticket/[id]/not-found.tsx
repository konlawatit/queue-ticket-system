import Link from "next/link";
import { KioskScreen } from "@/components/KioskScreen";
import { buttonStyles } from "@/components/styles";

export default function TicketNotFound() {
  return (
    <KioskScreen code="IT 05-2">
      <h1 className="text-2xl font-semibold">ไม่พบหมายเลขคิวนี้</h1>
      <p className="text-lg">ลิงก์อาจไม่ถูกต้อง กรุณารับบัตรคิวใหม่</p>
      <Link href="/" className={buttonStyles.primary}>
        กลับไปหน้ารับบัตรคิว
      </Link>
    </KioskScreen>
  );
}
