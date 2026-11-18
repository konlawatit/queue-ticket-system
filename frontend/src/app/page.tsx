import type { Metadata } from "next";
import Link from "next/link";
import { IssueTicketForm } from "@/components/IssueTicketForm";
import { KioskScreen } from "@/components/KioskScreen";
import { buttonStyles } from "@/components/styles";

export const metadata: Metadata = { title: "IT 05-1 รับบัตรคิว" };

/** IT 05-1: take a ticket, or go to the reset screen. */
export default function IssueTicketPage() {
  return (
    <KioskScreen code="IT 05-1">
      <IssueTicketForm />
      <Link href="/reset" className={`${buttonStyles.secondary} mt-auto`}>
        ล้างคิว
      </Link>
    </KioskScreen>
  );
}
