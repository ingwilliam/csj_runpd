"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";
import { LoaderCircle, RotateCcw } from "lucide-react";
import { Button } from "@/components/ui/button";

type RetryButtonProps = {
  label?: string;
  className?: string;
};

/** Vuelve a solicitar los datos al servidor sin recargar toda la página. */
export function RetryButton({ label = "Reintentar", className }: RetryButtonProps) {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  function reintentar() {
    startTransition(() => router.refresh());
  }

  return (
    <Button
      type="button"
      onClick={reintentar}
      disabled={isPending}
      className={className}
    >
      {isPending ? <LoaderCircle aria-hidden="true" className="animate-spin" /> : <RotateCcw aria-hidden="true" />}
      {isPending ? "Reintentando…" : label}
    </Button>
  );
}
