"use client";

import { motion, useReducedMotion } from "motion/react";
import { Eye, Pencil, ShieldCheck, UserCheck, UserX } from "lucide-react";
import { Button } from "@/components/ui/button";
import type { Usuario } from "@/lib/backend/types";
import {
  buildNombreCompleto,
  formatFechaCreacion,
  getIniciales,
} from "@/lib/usuarios/usuario-formato";
import { UserRoleChips, UserStatusBadge } from "./user-badges";

type UsersTableProps = {
  usuarios: Usuario[];
  usuarioActualId: string;
  onVer: (usuario: Usuario) => void;
  onEditar: (usuario: Usuario) => void;
  onRoles: (usuario: Usuario) => void;
  onCambiarActivo: (usuario: Usuario) => void;
};

/** Tabla de usuarios con las acciones de administración por fila. */
export function UsersTable({
  usuarios,
  usuarioActualId,
  onVer,
  onEditar,
  onRoles,
  onCambiarActivo,
}: UsersTableProps) {
  const shouldReduceMotion = useReducedMotion();

  return (
    <div className="overflow-hidden rounded-2xl border border-corporativo-azul/10 bg-white shadow-sm">
      <ul className="divide-y divide-slate-100">
        {usuarios.map((usuario, index) => {
          const esUsuarioActual = usuario.id === usuarioActualId;

          return (
            <motion.li
              key={usuario.id}
              initial={shouldReduceMotion ? false : { opacity: 0, x: -12 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.3, delay: Math.min(index * 0.03, 0.3) }}
              className="grid gap-3 p-4 transition-colors hover:bg-corporativo-azul/5 sm:grid-cols-[minmax(0,2fr)_minmax(0,1.5fr)_auto] sm:items-center"
            >
              <UserIdentity usuario={usuario} esUsuarioActual={esUsuarioActual} />
              <UserRoleChips roles={usuario.roles.map((rol) => rol.nombre)} />

              <div className="flex flex-wrap items-center gap-3 sm:justify-end">
                <UserStatusBadge activo={usuario.activo} />
                <span className="text-sm text-slate-500">
                  {formatFechaCreacion(usuario.fechaCreacionUtc)}
                </span>
                <UserActions
                  usuario={usuario}
                  esUsuarioActual={esUsuarioActual}
                  onVer={onVer}
                  onEditar={onEditar}
                  onRoles={onRoles}
                  onCambiarActivo={onCambiarActivo}
                />
              </div>
            </motion.li>
          );
        })}
      </ul>
    </div>
  );
}

/** Avatar con iniciales, nombre completo y correo del usuario. */
function UserIdentity({
  usuario,
  esUsuarioActual,
}: {
  usuario: Usuario;
  esUsuarioActual: boolean;
}) {
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
        <p className="truncate font-medium text-slate-800">
          {nombreCompleto}
          {esUsuarioActual && (
            <span className="ml-2 rounded-full bg-corporativo-amarillo/40 px-2 py-0.5 text-xs font-medium text-corporativo-azul">
              Tú
            </span>
          )}
        </p>
        <p className="truncate text-sm text-slate-500">{usuario.correoElectronico}</p>
      </div>
    </div>
  );
}

/** Botones de acción de una fila. */
function UserActions({
  usuario,
  esUsuarioActual,
  onVer,
  onEditar,
  onRoles,
  onCambiarActivo,
}: {
  usuario: Usuario;
  esUsuarioActual: boolean;
  onVer: (usuario: Usuario) => void;
  onEditar: (usuario: Usuario) => void;
  onRoles: (usuario: Usuario) => void;
  onCambiarActivo: (usuario: Usuario) => void;
}) {
  const nombre = buildNombreCompleto(usuario);
  const desactivacionBloqueada = esUsuarioActual && usuario.activo;

  return (
    <div className="flex items-center gap-1">
      <Button
        type="button"
        variant="ghost"
        size="icon-sm"
        title={`Ver detalle de ${nombre}`}
        aria-label={`Ver detalle de ${nombre}`}
        onClick={() => onVer(usuario)}
      >
        <Eye />
      </Button>
      <Button
        type="button"
        variant="ghost"
        size="icon-sm"
        title={`Editar a ${nombre}`}
        aria-label={`Editar a ${nombre}`}
        onClick={() => onEditar(usuario)}
      >
        <Pencil />
      </Button>
      <Button
        type="button"
        variant="ghost"
        size="icon-sm"
        title={`Cambiar roles de ${nombre}`}
        aria-label={`Cambiar roles de ${nombre}`}
        onClick={() => onRoles(usuario)}
      >
        <ShieldCheck />
      </Button>
      <Button
        type="button"
        variant="ghost"
        size="icon-sm"
        title={
          desactivacionBloqueada
            ? "No puedes desactivar tu propio usuario."
            : usuario.activo
              ? `Desactivar a ${nombre}`
              : `Activar a ${nombre}`
        }
        aria-label={
          usuario.activo
            ? `Desactivar a ${nombre}`
            : `Activar a ${nombre}`
        }
        disabled={desactivacionBloqueada}
        onClick={() => onCambiarActivo(usuario)}
        className={
          desactivacionBloqueada
            ? undefined
            : usuario.activo
              ? "text-red-600 hover:bg-red-50 hover:text-red-700"
              : "text-corporativo-verde hover:bg-corporativo-verde/10 hover:text-corporativo-verde"
        }
      >
        {usuario.activo ? <UserX /> : <UserCheck />}
      </Button>
    </div>
  );
}
