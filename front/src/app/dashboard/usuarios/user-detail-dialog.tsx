"use client";

import { useEffect, useState } from "react";
import { CalendarDays, LoaderCircle, Mail, Pencil } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { Usuario } from "@/lib/backend/types";
import {
  buildNombreCompleto,
  formatFechaCreacion,
  getIniciales,
} from "@/lib/usuarios/usuario-formato";
import { obtenerUsuarioAction } from "./actions";
import { UserRoleChips, UserStatusBadge } from "./user-badges";

type UserDetailDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  usuario: Usuario;
  onEditar: (usuario: Usuario) => void;
};

/** Diálogo con el detalle actualizado de un usuario consultado por su identificador. */
export function UserDetailDialog({
  open,
  onOpenChange,
  usuario,
  onEditar,
}: UserDetailDialogProps) {
  const [detalle, setDetalle] = useState(usuario);
  const [cargando, setCargando] = useState(true);

  // Al abrir, refresca el detalle con GET /api/usuarios/{id}.
  useEffect(() => {
    if (!open) {
      return;
    }

    let activo = true;

    void obtenerUsuarioAction(usuario.id).then((resultado) => {
      if (!activo) {
        return;
      }
      setCargando(false);
      if (resultado.ok) {
        setDetalle(resultado.usuario);
      } else {
        toast.error(resultado.message);
      }
    });

    return () => {
      activo = false;
    };
  }, [open, usuario.id]);

  const nombreCompleto = buildNombreCompleto(detalle);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            Detalle del usuario
            {cargando && (
              <LoaderCircle className="h-4 w-4 animate-spin text-slate-400" aria-label="Actualizando" />
            )}
          </DialogTitle>
          <DialogDescription>
            Información actual del usuario y sus roles asignados.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-5">
          <div className="flex items-center gap-4 rounded-2xl bg-corporativo-azul/5 p-4">
            <span
              aria-hidden="true"
              className="flex h-14 w-14 shrink-0 items-center justify-center rounded-full bg-linear-to-br from-corporativo-azul to-corporativo-verde text-lg font-semibold text-white"
            >
              {getIniciales(detalle)}
            </span>
            <div className="min-w-0 space-y-1">
              <p className="truncate text-lg font-semibold text-slate-800">{nombreCompleto}</p>
              <UserStatusBadge activo={detalle.activo} />
            </div>
          </div>

          <dl className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-1">
              <dt className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
                <Mail className="h-3.5 w-3.5" />
                Correo electrónico
              </dt>
              <dd className="text-sm break-all text-slate-800">{detalle.correoElectronico}</dd>
            </div>
            <div className="space-y-1">
              <dt className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
                <CalendarDays className="h-3.5 w-3.5" />
                Fecha de creación
              </dt>
              <dd className="text-sm text-slate-800">
                {formatFechaCreacion(detalle.fechaCreacionUtc)}
              </dd>
            </div>
          </dl>

          <div className="space-y-2">
            <p className="text-xs font-medium tracking-wide text-slate-500 uppercase">Roles</p>
            <UserRoleChips roles={detalle.roles.map((rol) => rol.nombre)} />
          </div>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button type="button" variant="outline" className="h-11">
              Cerrar
            </Button>
          </DialogClose>
          <Button
            type="button"
            onClick={() => onEditar(detalle)}
            className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
          >
            <Pencil />
            Editar usuario
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
