"use client";

import { usePathname } from "next/navigation";
import { useEffect, useRef } from "react";

/**
 * Mueve el foco al contenido principal cuando cambia la ruta.
 * En la primera carga no lo mueve para no interrumpir al usuario.
 */
export function RouteFocus() {
  const pathname = usePathname();
  const isFirstRender = useRef(true);

  useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }

    document.getElementById("contenido")?.focus({ preventScroll: true });
  }, [pathname]);

  return null;
}
