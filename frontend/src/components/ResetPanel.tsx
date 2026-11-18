"use client";

import { useActionState } from "react";
import { resetQueueAction, type ResetQueueState } from "@/app/actions";
import { buttonStyles, ticketCodeStyle } from "@/components/styles";

type ResetPanelProps = {
  /** Current code read on the server when the page was rendered. */
  initialCode: string;
};

export function ResetPanel({ initialCode }: ResetPanelProps) {
  const [state, formAction, isPending] = useActionState<ResetQueueState>(resetQueueAction, {
    code: initialCode,
    error: null,
  });

  return (
    <>
      <form action={formAction}>
        <button type="submit" disabled={isPending} aria-busy={isPending} className={buttonStyles.primary}>
          ล้างคิว
        </button>
      </form>
      <h1 className="text-2xl font-semibold">หมายเลขคิวปัจจุบัน</h1>
      <p data-testid="current-code" aria-live="polite" className={ticketCodeStyle}>
        {state.code}
      </p>
      {state.error && (
        <p role="alert" className="font-semibold text-red-700">
          {state.error}
        </p>
      )}
    </>
  );
}
