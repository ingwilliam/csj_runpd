"use client";

import { useActionState, useEffect } from "react";
import { cn } from "cn";
import { LoaderCircle, Save, ShieldCheck } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { Rol, Usuario } from "@/lib/backend/types";
import { ROLE_ADMINISTRADOR_DEL_SISTEMA } from "@/lib/auth/modules";
import { buildNombreCompleto } from "@/lib/usuarios/usuario-formato";
import { actualizarRolesAction, type UsuarioActionState } from "./actions";

const ESTADO_INICIAL: UsuarioActionState = { ok: false, message: null };

type UserRolesDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  roles: Rol[];
  usuario: Usuario;
  usuarioActualId: string;
  onCompleted: () => void;
};

/** Diálogo para cambiar únicamente los roles de un usuario. */
export function UserRolesDialog({
  open,
  onOpenChange,
  roles,
  usuario,
  usuarioActualId,
  onCompleted,
}: UserRolesDialogProps) {
  const [estado, accion, isPending] = useActionState(
    actualizarRolesAction,
    ESTADO_INICIAL,
  );

  // Notifica el resultado; al guardar correctamente se cierra el diálogo.
  useEffect(() => {
    if (!estado.message) {
      return;
    }
    if (estado.ok) {
      toast.success(estado.message);
      onCompleted();
    }
  }, [estado, onCompleted]);

  const rolesAsignados = new Set(usuario.roles.map((rol) => rol.id));
  const esUsuarioActual = usuario.id === usuarioActualId;
  const rolAdministrador = roles.find(
    (rol) => rol.nombre === ROLE_ADMINISTRADOR_DEL_SISTEMA,
  );
  const conservaRolAdministrador = rolAdministrador
    ? rolesAsignados.has(rolAdministrador.id)
    : false;

  return (
    <Dialog
      open={open}
      onOpenChange={(nuevoEstado) => {
        if (!isPending) {
          onOpenChange(nuevoEstado);
        }
      }}
    >
      <DialogContent>
        <DialogHeader>
          <span className="mb-1 flex h-10 w-10 items-center justify-center rounded-xl bg-corporativo-amarillo/30 text-corporativo-azul">
            <ShieldCheck className="h-5 w-5" />
          </span>
          <DialogTitle>Roles de {buildNombreCompleto(usuario)}</DialogTitle>
          <DialogDescription>
            Los roles seleccionados reemplazan a los actuales del usuario.
          </DialogDescription>
        </DialogHeader>

        <form action={accion} className="space-y-5">
          <input type="hidden" name="id" value={usuario.id} />

          {roles.length === 0 ? (
            <p className="rounded-xl border border-dashed border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-500">
              No hay roles activos disponibles.
            </p>
          ) : (
            <div className="grid gap-2">
              {roles.map((rol) => {
                const esRolAdministradorPropio =
                  esUsuarioActual && conservaRolAdministrador && rol.id === rolAdministrador?.id;

                return (
                  <label
                    key={rol.id}
                    className={cn(
                      "flex cursor-pointer items-start gap-3 rounded-xl border border-slate-200 bg-white p-3 transition-colors hover:border-corporativo-azul/30 hover:bg-corporativo-azul/5",
                      isPending && "cursor-not-allowed opacity-60",
                      esRolAdministradorPropio && "hover:border-slate-200 hover:bg-white",
                    )}
                  >
                    {esRolAdministradorPropio ? (
                      <>
                        <Checkbox defaultChecked disabled className="mt-0.5" />
                        <input type="hidden" name="roles" value={rol.id} />
                      </>
                    ) : (
                      <Checkbox
                        name="roles"
                        value={rol.id}
                        defaultChecked={rolesAsignados.has(rol.id)}
                        disabled={isPending}
                        className="mt-0.5"
                      />
                    )}
                    <span className="min-w-0 space-y-0.5">
                      <span className="block text-sm font-medium text-slate-800">
                        {rol.nombre}
                      </span>
                      {rol.descripcion && (
                        <span className="block text-xs text-slate-500">
                          {rol.descripcion}
                        </span>
                      )}
                    </span>
                  </label>
                );
              })}
            </div>
          )}

          {esUsuarioActual && (
            <p className="text-xs text-slate-500">
              No puedes quitarte el rol de {ROLE_ADMINISTRADOR_DEL_SISTEMA}.
            </p>
          )}

          {estado.message && !estado.ok && (
            <p
              role="alert"
              className="rounded-xl border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700"
            >
              {estado.message}
            </p>
          )}

          <DialogFooter>
            <DialogClose asChild>
              <Button type="button" variant="outline" disabled={isPending} className="h-11">
                Cancelar
              </Button>
            </DialogClose>
            <Button
              type="submit"
              disabled={isPending}
              className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
            >
              {isPending ? <LoaderCircle className="animate-spin" /> : <Save />}
              {isPending ? "Guardando..." : "Guardar roles"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
