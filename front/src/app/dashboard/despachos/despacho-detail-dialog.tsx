"use client";

import { useEffect, useState } from "react";
import {
  Building2,
  CalendarDays,
  LoaderCircle,
  MapPin,
  Pencil,
  Phone,
  Tags,
} from "lucide-react";
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
import type { CatalogosDespachos, Despacho, Municipio } from "@/lib/backend/types";
import { formatFechaCreacion } from "@/lib/despachos/despacho-formato";
import { obtenerDespachoAction, resolverMunicipioAction } from "./actions";
import {
  DespachoEstadoBadge,
  DespachoPromiscuoBadge,
  DespachoStatusBadge,
} from "./despacho-badges";

type DespachoDetailDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  despacho: Despacho;
  catalogos: CatalogosDespachos;
  puedeEditar: boolean;
  onEditar: (despacho: Despacho) => void;
};

/** Diálogo con el detalle actualizado de un despacho consultado por su identificador. */
export function DespachoDetailDialog({
  open,
  onOpenChange,
  despacho,
  catalogos,
  puedeEditar,
  onEditar,
}: DespachoDetailDialogProps) {
  const [detalle, setDetalle] = useState(despacho);
  const [municipio, setMunicipio] = useState<Municipio | null>(null);
  const [departamentoId, setDepartamentoId] = useState("");
  const [cargando, setCargando] = useState(true);

  // Al abrir, refresca el despacho y resuelve el nombre de su municipio.
  useEffect(() => {
    if (!open) {
      return;
    }

    let activo = true;

    void Promise.all([
      obtenerDespachoAction(despacho.id),
      resolverMunicipioAction(despacho.municipioId),
    ]).then(([resultadoDespacho, resultadoMunicipio]) => {
      if (!activo) {
        return;
      }
      setCargando(false);

      if (resultadoDespacho.ok) {
        setDetalle(resultadoDespacho.despacho);
      } else {
        toast.error(resultadoDespacho.message);
      }

      if (resultadoMunicipio.ok) {
        setMunicipio(resultadoMunicipio.municipio);
        setDepartamentoId(resultadoMunicipio.departamentoId);
      }
    });

    return () => {
      activo = false;
    };
  }, [open, despacho.id, despacho.municipioId]);

  const departamento = catalogos.departamentos.find(
    (item) => item.id === departamentoId,
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            Detalle del despacho
            {cargando && (
              <LoaderCircle
                className="h-4 w-4 animate-spin text-slate-400"
                aria-label="Actualizando"
              />
            )}
          </DialogTitle>
          <DialogDescription>
            Información actual del despacho, su ubicación y su clasificación.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-5">
          <div className="flex items-center gap-4 rounded-2xl bg-corporativo-azul/5 p-4">
            <span
              aria-hidden="true"
              className="flex h-14 w-14 shrink-0 items-center justify-center rounded-xl bg-corporativo-azul text-white"
            >
              <Building2 className="h-7 w-7" />
            </span>
            <div className="min-w-0 space-y-1">
              <p className="truncate text-lg font-semibold text-slate-800">
                {detalle.nombre}
              </p>
              <p className="truncate text-sm text-slate-500">{detalle.codigo}</p>
              <div className="flex flex-wrap gap-2 pt-1">
                <DespachoEstadoBadge estado={detalle.estado} />
                <DespachoStatusBadge activo={detalle.activo} />
                <DespachoPromiscuoBadge promiscuo={detalle.promiscuo} />
              </div>
            </div>
          </div>

          <section className="space-y-3">
            <h3 className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
              <MapPin className="h-3.5 w-3.5" />
              Ubicación
            </h3>
            <dl className="grid gap-3 sm:grid-cols-2">
              <DataItem etiqueta="Departamento" valor={departamento?.nombre} />
              <DataItem etiqueta="Municipio" valor={municipio?.nombre} />
              <DataItem
                etiqueta="Jurisdicción"
                valor={nombreDe(catalogos.jurisdicciones, detalle.jurisdiccionId)}
              />
              <DataItem
                etiqueta="Distrito"
                valor={nombreDe(catalogos.distritos, detalle.distritoId)}
              />
              <DataItem
                etiqueta="Circuito"
                valor={nombreDe(catalogos.circuitos, detalle.circuitoId)}
              />
            </dl>
          </section>

          <section className="space-y-3">
            <h3 className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
              <Tags className="h-3.5 w-3.5" />
              Clasificación
            </h3>
            <dl className="grid gap-3 sm:grid-cols-2">
              <DataItem
                etiqueta="Especialidad"
                valor={nombreDe(catalogos.especialidades, detalle.especialidadId)}
              />
              <DataItem
                etiqueta="Sub especialidad"
                valor={nombreDe(catalogos.subEspecialidades, detalle.subEspecialidadId)}
              />
              <DataItem
                etiqueta="Distrito de la sub especialidad"
                valor={nombreDe(
                  catalogos.distritos,
                  detalle.distritoSubEspecialidadId,
                )}
              />
              <DataItem
                etiqueta="Circuito de la sub especialidad"
                valor={nombreDe(
                  catalogos.circuitos,
                  detalle.circuitoSubEspecialidadId,
                )}
              />
              <DataItem
                etiqueta="Entidad"
                valor={nombreDe(catalogos.entidades, detalle.entidadId)}
              />
            </dl>
          </section>

          <section className="space-y-3">
            <h3 className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
              <Phone className="h-3.5 w-3.5" />
              Contacto
            </h3>
            <dl className="grid gap-3 sm:grid-cols-2">
              <DataItem etiqueta="Dirección" valor={detalle.direccion} />
              <DataItem
                etiqueta="Complemento de dirección"
                valor={detalle.complementoDireccion}
              />
              <DataItem etiqueta="Teléfono fijo" valor={detalle.telefonoFijo} />
              <DataItem etiqueta="Teléfono celular" valor={detalle.telefonoCelular} />
              <DataItem etiqueta="Correo" valor={detalle.correo} />
              <DataItem etiqueta="Horario de atención" valor={detalle.horarioAtencion} />
            </dl>
          </section>

          <section className="space-y-3">
            <h3 className="flex items-center gap-2 text-xs font-medium tracking-wide text-slate-500 uppercase">
              <CalendarDays className="h-3.5 w-3.5" />
              Registro
            </h3>
            <dl className="grid gap-3 sm:grid-cols-2">
              <DataItem
                etiqueta="Fecha de creación"
                valor={formatFechaCreacion(detalle.fechaCreacionUtc)}
              />
            </dl>
          </section>
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button type="button" variant="outline" className="h-11">
              Cerrar
            </Button>
          </DialogClose>
          {puedeEditar && (
            <Button
              type="button"
              onClick={() => onEditar(detalle)}
              className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
            >
              <Pencil />
              Editar despacho
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

/** Muestra un campo del detalle con su etiqueta. */
function DataItem({ etiqueta, valor }: { etiqueta: string; valor: string | null | undefined }) {
  return (
    <div className="space-y-1">
      <dt className="flex items-center gap-2 text-xs font-medium text-slate-500">
        {etiqueta}
      </dt>
      <dd className="text-sm break-words text-slate-800">{valor || "No aplica"}</dd>
    </div>
  );
}

/** Resuelve el nombre de un registro del catálogo por su identificador. */
function nombreDe(
  registros: { id: string; nombre: string }[],
  id: string | null,
): string | null {
  if (!id) {
    return null;
  }
  return registros.find((registro) => registro.id === id)?.nombre ?? id;
}
