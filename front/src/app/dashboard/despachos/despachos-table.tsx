"use client";

import { motion, useReducedMotion } from "motion/react";
import { Building2, Eye, Pencil, Power, PowerOff } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { Despacho } from "@/lib/backend/types";
import { formatFechaCreacion } from "@/lib/despachos/despacho-formato";
import {
  DespachoEstadoBadge,
  DespachoPromiscuoBadge,
  DespachoStatusBadge,
} from "./despacho-badges";

type DespachosTableProps = {
  despachos: Despacho[];
  puedeEditar: boolean;
  onVer: (despacho: Despacho) => void;
  onEditar: (despacho: Despacho) => void;
  onCambiarActivo: (despacho: Despacho) => void;
};

/** Tabla de despachos con las acciones de administración por fila. */
export function DespachosTable({
  despachos,
  puedeEditar,
  onVer,
  onEditar,
  onCambiarActivo,
}: DespachosTableProps) {
  const shouldReduceMotion = useReducedMotion();

  return (
    <div className="overflow-hidden rounded-2xl border border-corporativo-azul/10 bg-white shadow-sm">
      <ul className="divide-y divide-slate-100">
        {despachos.map((despacho, index) => (
          <motion.li
            key={despacho.id}
            initial={shouldReduceMotion ? false : { opacity: 0, x: -12 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.3, delay: Math.min(index * 0.03, 0.3) }}
            className="grid gap-3 p-4 transition-colors hover:bg-corporativo-azul/5 sm:grid-cols-[minmax(0,2fr)_minmax(0,1.5fr)_auto] sm:items-center"
          >
            <DespachoIdentity despacho={despacho} />
            <div className="flex flex-wrap items-center gap-2">
              <DespachoEstadoBadge estado={despacho.estado} />
              <DespachoStatusBadge activo={despacho.activo} />
              <DespachoPromiscuoBadge promiscuo={despacho.promiscuo} />
            </div>

            <div className="flex flex-wrap items-center gap-3 sm:justify-end">
              <span className="text-sm text-slate-500">
                {formatFechaCreacion(despacho.fechaCreacionUtc)}
              </span>
              <DespachoActions
                despacho={despacho}
                puedeEditar={puedeEditar}
                onVer={onVer}
                onEditar={onEditar}
                onCambiarActivo={onCambiarActivo}
              />
            </div>
          </motion.li>
        ))}
      </ul>
    </div>
  );
}

/** Identidad del despacho: icono, nombre y código. */
function DespachoIdentity({ despacho }: { despacho: Despacho }) {
  return (
    <div className="flex min-w-0 items-center gap-3">
      <span
        aria-hidden="true"
        className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-corporativo-azul text-white"
      >
        <Building2 className="h-5 w-5" />
      </span>
      <div className="min-w-0">
        <p className="truncate font-medium text-slate-800">{despacho.nombre}</p>
        <p className="truncate text-sm text-slate-500">{despacho.codigo}</p>
      </div>
    </div>
  );
}

/** Botones de acción de una fila. */
function DespachoActions({
  despacho,
  puedeEditar,
  onVer,
  onEditar,
  onCambiarActivo,
}: {
  despacho: Despacho;
  puedeEditar: boolean;
  onVer: (despacho: Despacho) => void;
  onEditar: (despacho: Despacho) => void;
  onCambiarActivo: (despacho: Despacho) => void;
}) {
  return (
    <div className="flex items-center gap-1">
      <Button
        type="button"
        variant="ghost"
        size="icon-sm"
        title={`Ver detalle de ${despacho.nombre}`}
        aria-label={`Ver detalle de ${despacho.nombre}`}
        onClick={() => onVer(despacho)}
      >
        <Eye />
      </Button>

      {puedeEditar && (
        <>
          <Button
            type="button"
            variant="ghost"
            size="icon-sm"
            title={`Editar ${despacho.nombre}`}
            aria-label={`Editar ${despacho.nombre}`}
            onClick={() => onEditar(despacho)}
          >
            <Pencil />
          </Button>
          <Button
            type="button"
            variant="ghost"
            size="icon-sm"
            title={
              despacho.activo
                ? `Desactivar ${despacho.nombre}`
                : `Activar ${despacho.nombre}`
            }
            aria-label={
              despacho.activo
                ? `Desactivar ${despacho.nombre}`
                : `Activar ${despacho.nombre}`
            }
            onClick={() => onCambiarActivo(despacho)}
            className={
              despacho.activo
                ? "text-red-600 hover:bg-red-50 hover:text-red-700"
                : "text-corporativo-verde hover:bg-corporativo-verde/10 hover:text-corporativo-verde"
            }
          >
            {despacho.activo ? <PowerOff /> : <Power />}
          </Button>
        </>
      )}
    </div>
  );
}
