"use client";

import { useMemo, useState } from "react";
import { motion, useReducedMotion } from "motion/react";
import { Search, Users } from "lucide-react";
import { Input } from "@/components/ui/input";
import type { Usuario } from "@/lib/backend/types";

const DATE_FORMATTER = new Intl.DateTimeFormat("es-CO", {
  dateStyle: "medium",
  // Fijar la zona evita diferencias entre el render del servidor y el del navegador.
  timeZone: "America/Bogota",
});

type UsersTableProps = {
  usuarios: Usuario[];
};

/** Tabla de usuarios con búsqueda en vivo por nombre, correo o rol. */
export function UsersTable({ usuarios }: UsersTableProps) {
  const [busqueda, setBusqueda] = useState("");
  const shouldReduceMotion = useReducedMotion();

  const usuariosFiltrados = useMemo(() => filterUsuarios(usuarios, busqueda), [usuarios, busqueda]);

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <h2 className="text-2xl font-bold text-corporativo-azul">Administración de usuarios</h2>
          <p className="text-sm text-slate-500">
            {usuarios.length} usuario{usuarios.length === 1 ? "" : "s"} registrado{usuarios.length === 1 ? "" : "s"}
          </p>
        </div>
        <div className="relative w-full sm:w-80">
          <Search className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
          <Input
            type="search"
            placeholder="Buscar por nombre, correo o rol"
            aria-label="Buscar usuarios"
            value={busqueda}
            onChange={(event) => setBusqueda(event.target.value)}
            className="h-11 bg-white pl-10"
          />
        </div>
      </div>

      {usuariosFiltrados.length === 0 ? (
        <div className="flex flex-col items-center gap-3 rounded-2xl border border-dashed border-corporativo-azul/30 bg-white p-10 text-center text-slate-500">
          <Users className="h-8 w-8 text-corporativo-azul/50" />
          <p>No se encontraron usuarios con ese criterio.</p>
        </div>
      ) : (
        <div className="overflow-hidden rounded-2xl border border-corporativo-azul/10 bg-white shadow-sm">
          <ul className="divide-y divide-slate-100">
            {usuariosFiltrados.map((usuario, index) => (
              <motion.li
                key={usuario.id}
                initial={shouldReduceMotion ? false : { opacity: 0, x: -12 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.3, delay: Math.min(index * 0.03, 0.3) }}
                className="grid gap-3 p-4 transition-colors hover:bg-corporativo-azul/5 sm:grid-cols-[minmax(0,2fr)_minmax(0,1.5fr)_auto] sm:items-center"
              >
                <UserIdentity usuario={usuario} />
                <RoleChips roles={usuario.roles.map((rol) => rol.nombre)} />
                <div className="flex items-center gap-3 text-sm sm:justify-end">
                  <StatusBadge activo={usuario.activo} />
                  <span className="text-slate-500">{formatDate(usuario.fechaCreacionUtc)}</span>
                </div>
              </motion.li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

/** Avatar con iniciales, nombre completo y correo del usuario. */
function UserIdentity({ usuario }: { usuario: Usuario }) {
  const nombreCompleto = buildNombreCompleto(usuario);
  return (
    <div className="flex min-w-0 items-center gap-3">
      <span
        aria-hidden="true"
        className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-linear-to-br from-corporativo-azul to-corporativo-verde text-sm font-semibold text-white"
      >
        {getIniciales(usuario)}
      </span>
      <div className="min-w-0">
        <p className="truncate font-medium text-slate-800">{nombreCompleto}</p>
        <p className="truncate text-sm text-slate-500">{usuario.correoElectronico}</p>
      </div>
    </div>
  );
}

/** Lista de roles del usuario como etiquetas. */
function RoleChips({ roles }: { roles: string[] }) {
  if (roles.length === 0) {
    return <span className="text-sm text-slate-400">Sin roles</span>;
  }
  return (
    <ul className="flex flex-wrap gap-2">
      {roles.map((rol) => (
        <li key={rol} className="rounded-full bg-corporativo-azul/10 px-2.5 py-0.5 text-xs font-medium text-corporativo-azul">
          {rol}
        </li>
      ))}
    </ul>
  );
}

/** Indicador de estado activo o inactivo con color y texto. */
function StatusBadge({ activo }: { activo: boolean }) {
  return activo ? (
    <span className="rounded-full bg-corporativo-verde/15 px-2.5 py-0.5 text-xs font-medium text-corporativo-verde">
      Activo
    </span>
  ) : (
    <span className="rounded-full bg-slate-200 px-2.5 py-0.5 text-xs font-medium text-slate-600">
      Inactivo
    </span>
  );
}

/**
 * Filtra por nombre, correo o nombre de rol, sin distinguir mayúsculas ni tildes.
 * Una búsqueda vacía devuelve todos los usuarios.
 */
function filterUsuarios(usuarios: Usuario[], busqueda: string): Usuario[] {
  const termino = normalizarTexto(busqueda.trim());
  if (!termino) {
    return usuarios;
  }
  return usuarios.filter((usuario) => {
    const textoBusqueda = [
      buildNombreCompleto(usuario),
      usuario.correoElectronico,
      ...usuario.roles.map((rol) => rol.nombre),
    ].join(" ");
    return normalizarTexto(textoBusqueda).includes(termino);
  });
}

/** Pasa a minúsculas y quita tildes para comparar textos en español. */
function normalizarTexto(texto: string): string {
  return texto.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
}

/** Une nombres y apellidos omitiendo los campos opcionales vacíos. */
function buildNombreCompleto(usuario: Usuario): string {
  return [usuario.primerNombre, usuario.segundoNombre, usuario.primerApellido, usuario.segundoApellido]
    .filter((parte): parte is string => Boolean(parte))
    .join(" ");
}

/** Iniciales del primer nombre y el primer apellido, para el avatar. */
function getIniciales(usuario: Usuario): string {
  return `${usuario.primerNombre.charAt(0)}${usuario.primerApellido.charAt(0)}`.toUpperCase();
}

/** Convierte la fecha UTC del backend a una fecha legible en Colombia. */
function formatDate(fechaUtc: string): string {
  return DATE_FORMATTER.format(new Date(fechaUtc));
}
