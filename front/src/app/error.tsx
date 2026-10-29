"use client";

import { useEffect } from "react";
import { ErrorPanel } from "@/components/error-panel";

type GlobalErrorProps = {
  error: unknown;
  retry: () => void;
};

/** Error inesperado fuera de una sección con su propio límite de errores. */
export default function GlobalError({ error, retry }: GlobalErrorProps) {
  useEffect(() => {
    console.error("Error inesperado en la aplicación:", error);
  }, [error]);

  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12">
      <div className="w-full max-w-xl">
        <ErrorPanel
          title="Algo salió mal"
          description="No pudimos cargar esta pantalla. Intenta de nuevo; si el problema continúa, vuelve al inicio."
          onRetry={retry}
          homeHref="/"
          homeLabel="Volver al inicio"
        />
      </div>
    </main>
  );
}
