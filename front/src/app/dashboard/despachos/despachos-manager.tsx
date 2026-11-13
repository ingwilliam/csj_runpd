"use client";

import Link from "next/link";
import { useCallback, useState, useTransition } from "react";
import { Building2, LoaderCircle, Plus } from "lucide-react";
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
  CatalogosDespachos,
  Despacho,
  DespachosPaginados,
  EstadoFiltroDespachos,
} from "@/lib/backend/types";
import { cambiarActivoDespachoAction } from "./actions";
import { DespachoDetailDialog } from "./despacho-detail-dialog";
import { DespachoFormDialog } from "./despacho-form-dialog";
import { buildDespachosHref } from "./despachos-query";
import { DespachosPagination } from "./despachos-pagination";
import { DespachosTable } from "./despachos-table";
import { DespachosToolbar } from "./despachos-toolbar";

type Dialogo =
  | { tipo: "crear" }
  | { tipo: "editar"; despacho: Despacho }
  | { tipo: "detalle"; despacho: Despacho };

type ConfirmacionActivo = {
  despacho: Despacho;
  activo: boolean;
};

type DespachosManagerProps = {
  resultado: DespachosPaginados;
  catalogos: CatalogosDespachos;
  puedeEditar: boolean;
  busqueda: string;
  estado: EstadoFiltroDespachos;
};

/** Administración de despachos: cabecera, tabla, paginación y diálogos de gestión. */
export function DespachosManager({
  resultado,
  catalogos,
  puedeEditar,
  busqueda,
  estado,
}: DespachosManagerProps) {
  const [dialogo, setDialogo] = useState<Dialogo | null>(null);
  const [confirmacion, setConfirmacion] = useState<ConfirmacionActivo | null>(null);
  const [isPending, startTransition] = useTransition();

  const {
    items: despachos,
    pagina,
    tamano,
    total,
    totalPaginas,
    totalDespachos,
    totalPermanentes,
    totalDescongestion,
    totalSuprimidos,
  } = resultado;

  const hayFiltro = busqueda.trim().length > 0 || estado !== "todos";
  const paginaFueraDeRango = despachos.length === 0 && total > 0 && pagina > totalPaginas;

  const cerrarDialogo = useCallback(() => setDialogo(null), []);

  function solicitarCambioActivo(despacho: Despacho) {
    setConfirmacion({ despacho, activo: !despacho.activo });
  }

  function confirmarCambioActivo() {
    if (!confirmacion) {
      return;
    }

    const { despacho, activo } = confirmacion;
    startTransition(async () => {
      const resultadoAccion = await cambiarActivoDespachoAction(despacho.id, activo);
      if (resultadoAccion.ok) {
        toast.success(resultadoAccion.message ?? "El despacho se actualizó correctamente.");
        setConfirmacion(null);
        return;
      }
      toast.error(resultadoAccion.message ?? "No se pudo completar la operación.");
    });
  }

  return (
    <div className="space-y-6">
      <DespachosToolbar
        busqueda={busqueda}
        estado={estado}
        totalDespachos={totalDespachos}
        totalPermanentes={totalPermanentes}
        totalDescongestion={totalDescongestion}
        totalSuprimidos={totalSuprimidos}
        puedeEditar={puedeEditar}
        onNuevoDespacho={() => setDialogo({ tipo: "crear" })}
      />

      {despachos.length === 0 ? (
        <div className="flex flex-col items-center gap-3 rounded-2xl border border-dashed border-corporativo-azul/30 bg-white p-10 text-center text-slate-500">
          <Building2 className="h-8 w-8 text-corporativo-azul/50" />
          <p>
            {totalDespachos === 0
              ? "Aún no hay despachos registrados."
              : total === 0
                ? "No se encontraron despachos con ese criterio."
                : "Esta página no tiene resultados."}
          </p>

          {totalDespachos === 0 && puedeEditar ? (
            <Button
              type="button"
              onClick={() => setDialogo({ tipo: "crear" })}
              className="bg-corporativo-azul text-white hover:bg-corporativo-azul/90"
            >
              <Plus />
              Crear primer despacho
            </Button>
          ) : paginaFueraDeRango ? (
            <Link
              href={buildDespachosHref({ pagina: 1, busqueda, estado })}
              className="text-sm font-medium text-corporativo-azul hover:underline"
            >
              Volver a la primera página
            </Link>
          ) : hayFiltro ? (
            <Link
              href={buildDespachosHref({ pagina: 1, busqueda: "", estado: "todos" })}
              className="text-sm font-medium text-corporativo-azul hover:underline"
            >
              Limpiar filtros
            </Link>
          ) : null}
        </div>
      ) : (
        <div className="space-y-3">
          <DespachosTable
            despachos={despachos}
            puedeEditar={puedeEditar}
            onVer={(despacho) => setDialogo({ tipo: "detalle", despacho })}
            onEditar={(despacho) => setDialogo({ tipo: "editar", despacho })}
            onCambiarActivo={solicitarCambioActivo}
          />

          <DespachosPagination
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
        <DespachoFormDialog
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          catalogos={catalogos}
          onCompleted={cerrarDialogo}
        />
      )}

      {dialogo?.tipo === "editar" && (
        <DespachoFormDialog
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          catalogos={catalogos}
          despacho={dialogo.despacho}
          onCompleted={cerrarDialogo}
        />
      )}

      {dialogo?.tipo === "detalle" && (
        <DespachoDetailDialog
          key={dialogo.despacho.id}
          open
          onOpenChange={(abierto) => {
            if (!abierto) {
              cerrarDialogo();
            }
          }}
          despacho={dialogo.despacho}
          catalogos={catalogos}
          puedeEditar={puedeEditar}
          onEditar={(despacho) => setDialogo({ tipo: "editar", despacho })}
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
              {confirmacion?.activo ? "¿Activar despacho?" : "¿Desactivar despacho?"}
            </AlertDialogTitle>
            <AlertDialogDescription>
              {confirmacion &&
                (confirmacion.activo
                  ? `${confirmacion.despacho.nombre} volverá a estar disponible en el listado.`
                  : `${confirmacion.despacho.nombre} dejará de estar disponible, pero sus datos se conservan.`)}
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
