"use client";

import { useEffect } from "react";
import { ErrorPanel } from "@/components/error-panel";

type DashboardErrorProps = {
  error: unknown;
  retry: () => void;
};

/** Error inesperado dentro del panel: conserva la cabecera y ofrece reintentar. */
export default function DashboardError({ error, retry }: DashboardErrorProps) {
  useEffect(() => {
    console.error("Error inesperado en el panel:", error);
  }, [error]);

  return (
    <div className="mx-auto max-w-2xl">
      <ErrorPanel
        title="No pudimos cargar esta sección"
        description="Ocurrió un problema al mostrar la información. Intenta de nuevo o vuelve al panel principal."
        onRetry={retry}
        homeHref="/dashboard"
        homeLabel="Volver al panel"
      />
    </div>
  );
}
