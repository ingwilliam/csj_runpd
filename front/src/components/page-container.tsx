import type { ReactNode } from "react";
import { cn } from "cn";

type PageContainerProps = {
  children: ReactNode;
  className?: string;
};

/**
 * Contenedor común de las pantallas privadas.
 * Fija el mismo ancho máximo y los mismos márgenes laterales para que la cabecera y el contenido queden alineados.
 */
export function PageContainer({ children, className }: PageContainerProps) {
  return (
    <div className={cn("mx-auto w-full max-w-6xl px-6 py-6", className)}>
      {children}
    </div>
  );
}
