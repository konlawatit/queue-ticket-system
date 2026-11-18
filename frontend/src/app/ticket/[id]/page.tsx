import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { KioskScreen } from "@/components/KioskScreen";
import { buttonStyles, ticketCodeStyle } from "@/components/styles";
import { formatIssuedAt, parseTicketId } from "@/lib/format";
import { getTicket } from "@/lib/queue-api";

export const metadata: Metadata = { title: "IT 05-2 หมายเลขคิว" };

/**
 * IT 05-2: shows one issued ticket. Reached by redirect after issuing, so
 * refreshing this page only reads the ticket again and never issues a new one.
 */
export default async function TicketPage({ params }: PageProps<"/ticket/[id]">) {
  const { id } = await params;
  const ticketId = parseTicketId(id);
  const ticket = ticketId === null ? null : await getTicket(ticketId);
  if (!ticket) {
    notFound();
  }

  const issuedAt = formatIssuedAt(ticket.issuedAt);

  return (
    <KioskScreen code="IT 05-2">
      <h1 className="text-2xl font-semibold">หมายเลขคิว</h1>
      <p data-testid="ticket-code" className={ticketCodeStyle}>
        {ticket.code}
      </p>
      <p data-testid="issued-at" className="text-lg">
        วันที่ : {issuedAt.date} เวลา {issuedAt.time} น.
      </p>
      <Link href="/" className={buttonStyles.primary}>
        กลับไปหน้ารับบัตรคิว
      </Link>
    </KioskScreen>
  );
}
