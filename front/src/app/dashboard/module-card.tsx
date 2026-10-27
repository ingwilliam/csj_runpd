"use client";

import Link from "next/link";
import { motion, useReducedMotion } from "motion/react";
import { ArrowRight, Users, type LucideIcon } from "lucide-react";
import type { AppModule } from "@/lib/auth/modules";

/** Icono de cada módulo, indexado por `AppModule.id`. Vive en el cliente porque los iconos no se pasan desde el servidor. */
const MODULE_ICONS: Record<string, LucideIcon> = {
  usuarios: Users,
};

type ModuleCardProps = {
  module: AppModule;
  /** Posición de la tarjeta; retrasa su entrada para que aparezcan en cascada. */
  index: number;
};

/** Tarjeta interactiva de un módulo: se eleva al pasar el cursor y muestra una flecha que avanza. */
export function ModuleCard({ module, index }: ModuleCardProps) {
  const shouldReduceMotion = useReducedMotion();
  const Icon = MODULE_ICONS[module.id] ?? Users;

  return (
    <motion.div
      initial={shouldReduceMotion ? false : { opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, delay: index * 0.08, ease: "easeOut" }}
      whileHover={shouldReduceMotion ? undefined : { y: -6 }}
      whileTap={shouldReduceMotion ? undefined : { scale: 0.98 }}
      className="h-full"
    >
      <Link
        href={module.href}
        className="group relative flex h-full flex-col gap-5 overflow-hidden rounded-2xl border border-corporativo-azul/10 bg-white p-6 shadow-sm transition-shadow hover:shadow-xl focus:outline-none focus-visible:ring-2 focus-visible:ring-corporativo-amarillo"
      >
        <div className="absolute inset-x-0 top-0 h-1 bg-linear-to-r from-corporativo-verde via-corporativo-amarillo to-corporativo-azul" />

        <span className="flex h-12 w-12 items-center justify-center rounded-xl bg-corporativo-azul text-white shadow-md transition-transform duration-300 group-hover:rotate-6 group-hover:scale-110">
          <Icon className="h-6 w-6" />
        </span>

        <div className="flex-1 space-y-1">
          <h3 className="text-lg font-semibold text-corporativo-azul">{module.title}</h3>
          <p className="text-sm text-slate-600">{module.description}</p>
        </div>

        <span className="flex items-center gap-1 text-sm font-medium text-corporativo-azul">
          Abrir
          <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
        </span>
      </Link>
    </motion.div>
  );
}
