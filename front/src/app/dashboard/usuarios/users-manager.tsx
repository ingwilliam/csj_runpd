"use client";

import Link from "next/link";
import { useCallback, useState, useTransition } from "react";
import { LoaderCircle, Plus, Users } from "lucide-react";
import { toast } from "sonner";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import type {
  EstadoFiltroUsuarios,
  Rol,
  Usuario,
  UsuariosPaginados,
} from "@/lib/backend/types";
import { buildNombreCompleto } from "@/lib/usuarios/usuario-formato";
import { cambiarActivoUsuarioAction } from "./actions";
import { UserDetailDialog } from "./user-detail-dialog";
import { UserFormDialog } from "./user-form-dialog";
import { UserRolesDialog } from "./user-roles-dialog";
import { buildUsuariosHref } from "./users-query";
import { UsersPagination } from "./users-pagination";
import { UsersTable } from "./users-table";
import { UsersToolbar } from "./users-toolbar";

type Dialogo =
  | { tipo: "crear" }
  | { tipo: "editar"; usuario: Usuario }
  | { tipo: "roles"; usuario: Usuario }
  | { tipo: "detalle"; usuario: Usuario };

type ConfirmacionActivo = {
  usuario: Usuario;
  activo: boolean;
};

type UsersManagerProps = {
  resultado: UsuariosPaginados;
  roles: Rol[];
  usuarioActualId: string;
  busqueda: string;
  estado: EstadoFiltroUsuarios;
};

/** Administración de usuarios: cabecera, tabla, paginación y diálogos de gestión. */
export function UsersManager({
  resultado,
  roles,
  usuarioActualId,
  busqueda,
  estado,
}: UsersManagerProps) {
  const [dialogo, setDialogo] = useState<Dialogo | null>(null);
  const [confirmacion, setConfirmacion] = useState<ConfirmacionActivo | null>(null);
  const [isPending, startTransition] = useTransition();

  const {
    items: usuarios,
    pagina,
    tamano,
    total,
    totalPaginas,
    totalUsuarios,
    totalActivos,
    totalInactivos,
  } = resultado;

  const hayFiltro = busqueda.trim().length > 0 || estado !== "todos";
  const paginaFueraDeRango = usuarios.length === 0 && total > 0 && pagina > totalPaginas;

  const cerrarDialogo = useCallback(() => setDialogo(null), []);

  function solicitarCambioActivo(usuario: Usuario) {
    const nuevoEstado = !usuario.activo;

    // El administrador no puede cerrarse su propia sesión desde esta pantalla.
    if (!nuevoEstado && usuario.id === usuarioActualId) {
      toast.error("No puedes desactivar tu propio usuario.");
      return;
    }

    setConfirmacion({ usuario, activo: nuevoEstado });
  }

  function confirmarCambioActivo() {
    if (!confirmacion) {
      return;
    }

    const { usuario, activo } = confirmacion;
    startTransition(async () => {
      const resultadoAccion = await cambiarActivoUsuarioAction(usuario.id, activo);
      if (resultadoAccion.ok) {
        toast.success(resultadoAccion.message ?? "El usuario se actualizó correctamente.");
        setConfirmacion(null);
        return;
      }
      toast.error(resultadoAccion.message ?? "No se pudo completar la operación.");
    });
  }

  return (
    <div className="space-y-6">
      <UsersToolbar
        busqueda={busqueda}
        estado={estado}
        totalUsuarios={totalUsuarios}
        totalActivos={totalActivos}
        totalInactivos={totalInactivos}
        onNuevoUsuario={() => setDialogo({ tipo: "crear" })}
      />

      {usuarios.length === 0 ? (
        <div className="flex flex-col items-center gap-3 rounded-2xl border border-dashed border-corporativo-azul/30 bg-white p-10 text-center text-slate-500">
          <Users className="h-8 w-8 text-corporativo-azul/50" />
          <p>
            {totalUsuarios === 0
              ? "Aún no hay usuarios registrados."
              : total === 0
                ? "No se encontraron usuarios con ese criterio."
                : "Esta página no tiene resultados."}
          </p>

          {totalUsuarios === 0 ? (
            <Button
              type="button"
              onClick={() => setDialogo({ tipo: "crear" })}
              className="bg-corporativo-azul text-white hover:bg-corporativo-azul/90"
            >
              <Plus />
              Crear primer usuario
            </Button>
          ) : paginaFueraDeRango ? (
            <Link
              href={buildUsuariosHref({ pagina: 1, busqueda, estado })}
              className="text-sm font-medium text-corporativo-azul hover:underline"
            >
              Volver a la primera página
            </Link>
          ) : hayFiltro ? (
            <Link
              href={buildUsuariosHref({ pagina: 1, busqueda: "", estado: "todos" })}
              className="text-sm font-medium text-corporativo-azul hover:underline"
            >
              Limpiar filtros
            </Link>
          ) : null}
        </div>
      ) : (
        <div className="space-y-3">
          <UsersTable
            usuarios={usuarios}
            usuarioActualId={usuarioActualId}
            onVer={(usuario) => setDialogo({ tipo: "detalle", usuario })}
            onEditar={(usuario) => setDialogo({ tipo: "editar", usuario })}
            onRoles={(usuario) => setDialogo({ tipo: "roles", usuario })}
            onCambiarActivo={solicitarCambioActivo}
          />

          <UsersPagination
            pagina={pagina}
            tamano={tamano}
            total={total}
            totalPaginas={totalPaginas}
            busqueda={busqueda}
            estado={estado}
          />
        </div>
      )}

      {dialogo?.tipo === "crear" && (
        <UserFormDialog
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          roles={roles}
          usuarioActualId={usuarioActualId}
          onCompleted={cerrarDialogo}
        />
      )}

      {dialogo?.tipo === "editar" && (
        <UserFormDialog
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          roles={roles}
          usuario={dialogo.usuario}
          usuarioActualId={usuarioActualId}
          onCompleted={cerrarDialogo}
        />
      )}

      {dialogo?.tipo === "roles" && (
        <UserRolesDialog
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          roles={roles}
          usuario={dialogo.usuario}
          usuarioActualId={usuarioActualId}
          onCompleted={cerrarDialogo}
        />
      )}

      {dialogo?.tipo === "detalle" && (
        <UserDetailDialog
          key={dialogo.usuario.id}
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          usuario={dialogo.usuario}
          onEditar={(usuario) => setDialogo({ tipo: "editar", usuario })}
        />
      )}

      <AlertDialog
        open={confirmacion !== null}
        onOpenChange={(abierto) => {
          if (!abierto && !isPending) {
            setConfirmacion(null);
          }
        }}
      >
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              {confirmacion?.activo ? "¿Activar usuario?" : "¿Desactivar usuario?"}
            </AlertDialogTitle>
            <AlertDialogDescription>
              {confirmacion &&
                (confirmacion.activo
                  ? `${buildNombreCompleto(confirmacion.usuario)} podrá volver a iniciar sesión.`
                  : `${buildNombreCompleto(confirmacion.usuario)} no podrá iniciar sesión, pero sus datos y roles se conservan.`)}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={isPending}>Cancelar</AlertDialogCancel>
            <AlertDialogAction
              disabled={isPending}
              onClick={(event) => {
                event.preventDefault();
                confirmarCambioActivo();
              }}
              className={
                confirmacion?.activo
                  ? undefined
                  : "bg-red-600 text-white hover:bg-red-700 focus-visible:ring-red-300"
              }
            >
              {isPending && <LoaderCircle className="animate-spin" />}
              {confirmacion?.activo ? "Sí, activar" : "Sí, desactivar"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
