import { LoaderCircle, MapPin } from "lucide-react";
import type {
  CatalogosDespachos,
  ResolverTerritorioResponse,
} from "@/lib/backend/types";

type DespachoTerritorioCardProps = {
  catalogos: CatalogosDespachos;
  cargando: boolean;
  error: string | null;
  territorio: ResolverTerritorioResponse | null;
  jurisdiccionId: string;
  municipioId: string;
  subEspecialidadId: string;
};

/**
 * Muestra la clasificación territorial que el backend resuelve para la
 * jurisdicción, el municipio y la sub especialidad seleccionados.
 */
export function DespachoTerritorioCard({
  catalogos,
  cargando,
  error,
  territorio,
  jurisdiccionId,
  municipioId,
  subEspecialidadId,
}: DespachoTerritorioCardProps) {
  const tieneSeleccion = jurisdiccionId !== "" && municipioId !== "";

  return (
    <section className="space-y-3 rounded-2xl border border-corporativo-azul/15 bg-corporativo-azul/5 p-4">
      <div className="flex items-center justify-between gap-2">
        <h3 className="flex items-center gap-2 text-sm font-semibold text-corporativo-azul">
          <MapPin className="h-4 w-4" />
          Información territorial
        </h3>
        {cargando && (
          <span className="flex items-center gap-1.5 text-xs text-slate-500">
            <LoaderCircle className="h-3.5 w-3.5 animate-spin" aria-hidden="true" />
            Consultando…
          </span>
        )}
      </div>

      {!tieneSeleccion ? (
        <p className="text-sm text-slate-500">
          Selecciona una jurisdicción y un municipio para consultar la clasificación territorial.
        </p>
      ) : error ? (
        <p role="alert" className="text-sm text-red-700">
          {error}
        </p>
      ) : cargando ? (
        <p className="text-sm text-slate-500">Consultando la clasificación territorial…</p>
      ) : territorio ? (
        <div className="space-y-3">
          <div className="grid gap-3 sm:grid-cols-2">
            <ClasificacionBloque
              titulo="Clasificación general"
              clasificacion={territorio.general}
              catalogos={catalogos}
              vacio="No hay clasificación general registrada para este municipio."
            />

            {subEspecialidadId !== "" && (
              <ClasificacionBloque
                titulo="Clasificación específica"
                clasificacion={territorio.especifico}
                catalogos={catalogos}
                vacio="No hay clasificación específica para esta sub especialidad."
                subEspecialidadId={subEspecialidadId}
              />
            )}
          </div>          
        </div>
      ) : null}
    </section>
  );
}

type ClasificacionBloqueProps = {
  titulo: string;
  clasificacion: ResolverTerritorioResponse["general"];
  catalogos: CatalogosDespachos;
  vacio: string;
  subEspecialidadId?: string;
};

/** Muestra el distrito y el circuito de una clasificación territorial. */
function ClasificacionBloque({
  titulo,
  clasificacion,
  catalogos,
  vacio,
  subEspecialidadId,
}: ClasificacionBloqueProps) {
  return (
    <div className="space-y-2 rounded-xl border border-slate-200 bg-white px-3 py-3">
      <p className="text-xs font-semibold tracking-wide text-slate-500 uppercase">{titulo}</p>

      {!clasificacion ? (
        <p className="text-sm text-slate-500">{vacio}</p>
      ) : (
        <dl className="space-y-1.5 text-sm">
          {subEspecialidadId && (
            <div className="flex gap-1.5">
              <dt className="shrink-0 text-slate-500">Sub especialidad:</dt>
              <dd className="text-slate-800">
                {nombreDe(catalogos.subEspecialidades, subEspecialidadId)}
              </dd>
            </div>
          )}
          <div className="flex gap-1.5">
            <dt className="shrink-0 text-slate-500">Distrito:</dt>
            <dd className="text-slate-800">{nombreDe(catalogos.distritos, clasificacion.distritoId)}</dd>
          </div>
          <div className="flex gap-1.5">
            <dt className="shrink-0 text-slate-500">Circuito:</dt>
            <dd className="text-slate-800">
              {clasificacion.circuitoId
                ? nombreDe(catalogos.circuitos, clasificacion.circuitoId)
                : "No asignado"}
            </dd>
          </div>
        </dl>
      )}
    </div>
  );
}

/** Resuelve el nombre de un registro del catálogo por su identificador. */
function nombreDe(
  registros: { id: string; nombre: string }[],
  id: string | null,
): string {
  if (!id) {
    return "No asignado";
  }
  return registros.find((registro) => registro.id === id)?.nombre ?? id;
}
