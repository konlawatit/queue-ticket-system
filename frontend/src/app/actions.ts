"use server";

import { redirect } from "next/navigation";
import { issueTicket, resetQueue } from "@/lib/queue-api";

export type IssueTicketState = { error: string | null };

export type ResetQueueState = { code: string; error: string | null };

const QUEUE_FULL_MESSAGE = "คิวเต็มแล้ว (ออกบัตรถึง Z9) กรุณาล้างคิวก่อนรับบัตรใหม่";
const UNAVAILABLE_MESSAGE = "ระบบขัดข้อง กรุณาลองใหม่อีกครั้ง";

/** IT 05-1: issue a ticket, then redirect to its page (Post/Redirect/Get, so a refresh never issues another). */
export async function issueTicketAction(): Promise<IssueTicketState> {
  let ticketId: number;
  try {
    const result = await issueTicket();
    if (result.kind === "full") {
      return { error: QUEUE_FULL_MESSAGE };
    }
    ticketId = result.ticket.id;
  } catch (error) {
    console.error("Issuing a ticket failed", error);
    return { error: UNAVAILABLE_MESSAGE };
  }

  // Outside the try: redirect() works by throwing, which the catch above would swallow.
  redirect(`/ticket/${ticketId}`);
}

/** IT 05-3: start a new round and show "00". */
export async function resetQueueAction(previous: ResetQueueState): Promise<ResetQueueState> {
  try {
    const { code } = await resetQueue();
    return { code, error: null };
  } catch (error) {
    console.error("Resetting the queue failed", error);
    return { code: previous.code, error: UNAVAILABLE_MESSAGE };
  }
}
