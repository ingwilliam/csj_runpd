"use client";

import { motion, useReducedMotion } from "motion/react";
import { Sparkles } from "lucide-react";

/** Banner de bienvenida del panel: degradado institucional con una entrada animada. */
export function WelcomeBanner() {
  const shouldReduceMotion = useReducedMotion();

  return (
    <motion.section
      initial={shouldReduceMotion ? false : { opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, ease: "easeOut" }}
      className="relative overflow-hidden rounded-3xl bg-linear-to-br from-corporativo-azul via-corporativo-azul to-corporativo-verde p-8 text-white shadow-xl"
    >
      {/* Formas decorativas: no aportan contenido, por eso se ocultan a los lectores de pantalla. */}
      <div aria-hidden="true" className="absolute -top-16 -right-10 h-56 w-56 rounded-full bg-corporativo-amarillo/20 blur-3xl" />
      <div aria-hidden="true" className="absolute -bottom-20 left-1/3 h-48 w-48 rounded-full bg-white/10 blur-2xl" />

      <div className="relative flex items-center gap-5">
        <span className="flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl bg-corporativo-amarillo text-corporativo-azul shadow-lg">
          <Sparkles className="h-7 w-7" />
        </span>
        <div className="space-y-1">
          <h2 className="text-2xl font-bold tracking-tight">¡Bienvenido al RUNPD!</h2>
          <p className="text-white/80">
            Has iniciado sesión correctamente. Elige un módulo para comenzar.
          </p>
        </div>
      </div>
    </motion.section>
  );
}
