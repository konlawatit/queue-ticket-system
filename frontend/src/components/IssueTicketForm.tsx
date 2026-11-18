"use client";

import { useActionState } from "react";
import { issueTicketAction, type IssueTicketState } from "@/app/actions";
import { buttonStyles } from "@/components/styles";

const initialState: IssueTicketState = { error: null };

/**
 * The button is disabled while the request is in flight, so one person's double
 * click sends a single request. Requests from several kiosks at once are made safe
 * by the API (database row lock), not by this button.
 */
export function IssueTicketForm() {
  const [state, formAction, isPending] = useActionState(issueTicketAction, initialState);

  return (
    <form action={formAction} className="flex flex-col items-center gap-4">
      <button type="submit" disabled={isPending} aria-busy={isPending} className={buttonStyles.hero}>
        รับบัตรคิว
      </button>
      {state.error && (
        <p role="alert" className="font-semibold text-red-700">
          {state.error}
        </p>
      )}
    </form>
  );
}
