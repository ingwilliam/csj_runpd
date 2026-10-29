import Link from "next/link";
import { TriangleAlert } from "lucide-react";
import { Button } from "@/components/ui/button";

type ErrorPanelProps = {
  title: string;
  description: string;
  onRetry?: () => void;
  retryLabel?: string;
  homeHref?: string;
  homeLabel?: string;
};

/** Tarjeta de error amigable con acciones para reintentar o volver a una pantalla segura. */
export function ErrorPanel({
  title,
  description,
  onRetry,
  retryLabel = "Reintentar",
  homeHref,
  homeLabel = "Volver al inicio",
}: ErrorPanelProps) {
  return (
    <div
      role="alert"
      className="rounded-2xl border border-red-200 bg-white p-8 text-center shadow-sm"
    >
      <span className="mx-auto flex h-14 w-14 items-center justify-center rounded-full bg-red-50 text-red-600">
        <TriangleAlert aria-hidden="true" className="h-7 w-7" />
      </span>

      <h1 className="mt-4 text-xl font-bold text-slate-800">{title}</h1>
      <p className="mt-2 text-sm text-slate-600">{description}</p>

      <div className="mt-6 flex flex-col justify-center gap-3 sm:flex-row">
        {onRetry && (
          <Button
            type="button"
            onClick={onRetry}
            className="h-11 bg-corporativo-azul px-5 text-white hover:bg-corporativo-azul/90"
          >
            {retryLabel}
          </Button>
        )}

        {homeHref && (
          <Button asChild variant="outline" className="h-11 px-5">
            <Link href={homeHref}>{homeLabel}</Link>
          </Button>
        )}
      </div>
    </div>
  );
}
