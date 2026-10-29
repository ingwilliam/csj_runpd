"use client";

import { useActionState, useEffect, type ReactNode } from "react";
import { cn } from "cn";
import { LoaderCircle, Pencil, Save, UserPlus } from "lucide-react";
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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { Rol, Usuario } from "@/lib/backend/types";
import { ROLE_ADMINISTRADOR_DEL_SISTEMA } from "@/lib/auth/modules";
import { buildNombreCompleto } from "@/lib/usuarios/usuario-formato";
import {
  actualizarUsuarioAction,
  crearUsuarioAction,
  type UsuarioActionState,
} from "./actions";

const ESTADO_INICIAL: UsuarioActionState = { ok: false, message: null };

type UserFormDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  roles: Rol[];
  usuario?: Usuario;
  usuarioActualId: string;
  onCompleted: () => void;
};

/** Diálogo para crear o editar un usuario, incluyendo sus roles en la misma petición. */
export function UserFormDialog({
  open,
  onOpenChange,
  roles,
  usuario,
  usuarioActualId,
  onCompleted,
}: UserFormDialogProps) {
  const esEdicion = usuario !== undefined;
  const [estado, accion, isPending] = useActionState(
    esEdicion ? actualizarUsuarioAction : crearUsuarioAction,
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

  const titulo = esEdicion ? "Editar usuario" : "Nuevo usuario";
  const descripcion = esEdicion
    ? `Actualiza los datos y los roles de ${buildNombreCompleto(usuario)}.`
    : "Registra un usuario y define sus roles de acceso.";
  const rolesAsignados = new Set(usuario?.roles.map((rol) => rol.id));
  const esUsuarioActual = usuario?.id === usuarioActualId;
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
            {esEdicion ? <Pencil className="h-5 w-5" /> : <UserPlus className="h-5 w-5" />}
          </span>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>{descripcion}</DialogDescription>
        </DialogHeader>

        <form action={accion} className="space-y-5">
          {usuario && <input type="hidden" name="id" value={usuario.id} />}

          <div className="grid gap-4 sm:grid-cols-2">
            <Field label="Primer nombre" htmlFor="usuario-primerNombre" required>
              <Input
                id="usuario-primerNombre"
                name="primerNombre"
                defaultValue={usuario?.primerNombre ?? ""}
                maxLength={100}
                required
                autoFocus
                disabled={isPending}
                className="h-11"
              />
            </Field>
            <Field label="Segundo nombre" htmlFor="usuario-segundoNombre">
              <Input
                id="usuario-segundoNombre"
                name="segundoNombre"
                defaultValue={usuario?.segundoNombre ?? ""}
                maxLength={100}
                disabled={isPending}
                className="h-11"
              />
            </Field>
            <Field label="Primer apellido" htmlFor="usuario-primerApellido" required>
              <Input
                id="usuario-primerApellido"
                name="primerApellido"
                defaultValue={usuario?.primerApellido ?? ""}
                maxLength={100}
                required
                disabled={isPending}
                className="h-11"
              />
            </Field>
            <Field label="Segundo apellido" htmlFor="usuario-segundoApellido">
              <Input
                id="usuario-segundoApellido"
                name="segundoApellido"
                defaultValue={usuario?.segundoApellido ?? ""}
                maxLength={100}
                disabled={isPending}
                className="h-11"
              />
            </Field>
          </div>

          <Field label="Correo electrónico" htmlFor="usuario-correoElectronico" required>
            <Input
              id="usuario-correoElectronico"
              name="correoElectronico"
              type="email"
              defaultValue={usuario?.correoElectronico ?? ""}
              maxLength={254}
              required
              autoComplete="email"
              disabled={isPending}
              className="h-11"
            />
          </Field>

          {!esEdicion && (
            <Field
              label="Contraseña inicial"
              htmlFor="usuario-contrasena"
              required
              hint="Debe tener entre 8 y 100 caracteres."
            >
              <Input
                id="usuario-contrasena"
                name="contrasena"
                type="password"
                minLength={8}
                maxLength={100}
                required
                autoComplete="new-password"
                disabled={isPending}
                className="h-11"
              />
            </Field>
          )}

          <fieldset className="space-y-3">
            <legend className="text-sm font-medium text-slate-800">Roles</legend>
            <p className="text-xs text-slate-500">
              Opcional. Define los módulos a los que podrá acceder el usuario.
            </p>

            {roles.length === 0 ? (
              <p className="rounded-xl border border-dashed border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-500">
                No hay roles activos disponibles.
              </p>
            ) : (
              <div className="grid gap-2 sm:grid-cols-2">
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
          </fieldset>

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
              {isPending ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear usuario"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

/** Etiqueta, control y texto de ayuda de un campo del formulario. */
function Field({
  label,
  htmlFor,
  required = false,
  hint,
  children,
}: {
  label: string;
  htmlFor: string;
  required?: boolean;
  hint?: string;
  children: ReactNode;
}) {
  return (
    <div className="space-y-2">
      <Label htmlFor={htmlFor}>
        {label}
        {required && <span className="text-red-500"> *</span>}
      </Label>
      {children}
      {hint && <p className="text-xs text-slate-500">{hint}</p>}
    </div>
  );
}
