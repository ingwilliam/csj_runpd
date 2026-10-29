"use client";

import { type ChangeEvent, useEffect, useRef, useTransition } from "react";
import { useRouter } from "next/navigation";
import { LoaderCircle, Plus, Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { EstadoFiltroUsuarios } from "@/lib/backend/types";
import { buildUsuariosHref } from "./users-query";

type UsersToolbarProps = {
  busqueda: string;
  estado: EstadoFiltroUsuarios;
  totalUsuarios: number;
  totalActivos: number;
  totalInactivos: number;
  onNuevoUsuario: () => void;
};

/**
 * Cabecera de la administración de usuarios: resumen, búsqueda y filtros.
 * La búsqueda y el estado se reflejan en la URL para conservarlos al navegar.
 */
export function UsersToolbar({
  busqueda,
  estado,
  totalUsuarios,
  totalActivos,
  totalInactivos,
  onNuevoUsuario,
}: UsersToolbarProps) {
  const router = useRouter();
  const inputRef = useRef<HTMLInputElement>(null);
  const timerRef = useRef<number | undefined>(undefined);
  const [isPending, startTransition] = useTransition();

  // Alinea el campo cuando la URL cambia por atrás/adelante, sin pisar lo que se escribe.
  useEffect(() => {
    const input = inputRef.current;
    if (!input || document.activeElement === input) {
      return;
    }
    if (input.value !== busqueda) {
      input.value = busqueda;
    }
  }, [busqueda]);

  // Cancela la búsqueda pendiente si el componente se desmonta.
  useEffect(() => {
    return () => window.clearTimeout(timerRef.current);
  }, []);

  // Aplica la búsqueda después de una pausa para no consultar en cada tecla.
  function manejarCambio(event: ChangeEvent<HTMLInputElement>) {
    window.clearTimeout(timerRef.current);
    const termino = event.target.value.trim();
    if (termino === busqueda) {
      return;
    }

    timerRef.current = window.setTimeout(() => {
      startTransition(() => {
        router.replace(
          buildUsuariosHref({ pagina: 1, busqueda: termino, estado }),
          { scroll: false },
        );
      });
    }, 350);
  }

  function cambiarEstado(nuevoEstado: EstadoFiltroUsuarios) {
    window.clearTimeout(timerRef.current);
    const termino = (inputRef.current?.value ?? busqueda).trim();

    startTransition(() => {
      router.replace(
        buildUsuariosHref({ pagina: 1, busqueda: termino, estado: nuevoEstado }),
        { scroll: false },
      );
    });
  }

  return (
    <div className="space-y-4">
      <header className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold text-corporativo-azul">Administración de usuarios</h1>
          <p className="text-sm text-slate-500">
            {totalUsuarios} usuario{totalUsuarios === 1 ? "" : "s"} registrado
            {totalUsuarios === 1 ? "" : "s"} · {totalActivos} activo
            {totalActivos === 1 ? "" : "s"}
          </p>
        </div>

        <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
          <div className="relative w-full sm:w-72">
            <Search className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <Input
              ref={inputRef}
              type="search"
              maxLength={100}
              defaultValue={busqueda}
              placeholder="Buscar por nombre, correo o rol"
              aria-label="Buscar usuarios"
              aria-busy={isPending}
              onChange={manejarCambio}
              className="h-11 bg-white pr-10 pl-10"
            />
            {isPending && (
              <LoaderCircle
                aria-hidden="true"
                className="absolute top-1/2 right-3 h-4 w-4 -translate-y-1/2 animate-spin text-corporativo-azul"
              />
            )}
          </div>
          <Button
            type="button"
            onClick={onNuevoUsuario}
            className="h-11 bg-corporativo-azul px-5 text-white hover:bg-corporativo-azul/90"
          >
            <Plus />
            Nuevo usuario
          </Button>
        </div>
      </header>

      <div
        className="flex flex-wrap gap-2"
        role="group"
        aria-label="Filtrar usuarios por estado"
        aria-busy={isPending}
      >
        <FilterButton
          activo={estado === "todos"}
          etiqueta="Todos"
          cantidad={totalUsuarios}
          onClick={() => cambiarEstado("todos")}
        />
        <FilterButton
          activo={estado === "activos"}
          etiqueta="Activos"
          cantidad={totalActivos}
          onClick={() => cambiarEstado("activos")}
        />
        <FilterButton
          activo={estado === "inactivos"}
          etiqueta="Inactivos"
          cantidad={totalInactivos}
          onClick={() => cambiarEstado("inactivos")}
        />
      </div>
    </div>
  );
}

/** Botón de filtro por estado con su cantidad de usuarios. */
function FilterButton({
  activo,
  etiqueta,
  cantidad,
  onClick,
}: {
  activo: boolean;
  etiqueta: string;
  cantidad: number;
  onClick: () => void;
}) {
  return (
    <button
      type="button"
      aria-pressed={activo}
      onClick={onClick}
      className={
        activo
          ? "flex items-center gap-2 rounded-full bg-corporativo-azul px-4 py-2 text-sm font-medium text-white"
          : "flex items-center gap-2 rounded-full border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-600 transition-colors hover:border-corporativo-azul/30 hover:text-corporativo-azul"
      }
    >
      {etiqueta}
      <span
        className={
          activo
            ? "rounded-full bg-white/20 px-2 py-0.5 text-xs"
            : "rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-500"
        }
      >
        {cantidad}
      </span>
    </button>
  );
}
