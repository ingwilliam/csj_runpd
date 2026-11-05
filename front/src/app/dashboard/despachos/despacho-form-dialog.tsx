"use client";

import {
  useActionState,
  useEffect,
  useState,
  type ComponentProps,
  type ReactNode,
} from "react";
import { cn } from "cn";
import { Building2, LoaderCircle, Pencil, Save } from "lucide-react";
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
import type { CatalogosDespachos, Despacho, Municipio } from "@/lib/backend/types";
import { ESTADOS_DESPACHO } from "@/lib/despachos/despacho-formato";
import {
  actualizarDespachoAction,
  crearDespachoAction,
  obtenerMunicipiosAction,
  resolverMunicipioAction,
  type DespachoActionState,
} from "./actions";

const ESTADO_INICIAL: DespachoActionState = { ok: false, message: null };

type DespachoFormDialogProps = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  catalogos: CatalogosDespachos;
  despacho?: Despacho;
  onCompleted: () => void;
};

/**
 * Diálogo para crear o editar un despacho con sus datos de ubicación,
 * clasificación y contacto.
 */
export function DespachoFormDialog({
  open,
  onOpenChange,
  catalogos,
  despacho,
  onCompleted,
}: DespachoFormDialogProps) {
  const esEdicion = despacho !== undefined;
  const [estado, accion, isPending] = useActionState(
    esEdicion ? actualizarDespachoAction : crearDespachoAction,
    ESTADO_INICIAL,
  );

  const [departamentoId, setDepartamentoId] = useState("");
  const [municipioId, setMunicipioId] = useState(despacho?.municipioId ?? "");
  const [municipios, setMunicipios] = useState<Municipio[]>([]);
  const [municipioActual, setMunicipioActual] = useState<Municipio | null>(null);
  const [cargandoMunicipios, setCargandoMunicipios] = useState(false);
  const [resolviendoMunicipio, setResolviendoMunicipio] = useState(esEdicion);
  const [jurisdiccionId, setJurisdiccionId] = useState(despacho?.jurisdiccionId ?? "");
  const [distritoId, setDistritoId] = useState(despacho?.distritoId ?? "");
  const [circuitoId, setCircuitoId] = useState(despacho?.circuitoId ?? "");
  const [especialidadId, setEspecialidadId] = useState(despacho?.especialidadId ?? "");
  const [subEspecialidadId, setSubEspecialidadId] = useState(
    despacho?.subEspecialidadId ?? "",
  );
  const [entidadId, setEntidadId] = useState(despacho?.entidadId ?? "");

  // Al editar, resuelve el departamento del municipio actual para poblar los selectores.
  useEffect(() => {
    if (!open || !despacho) {
      return;
    }

    let activo = true;

    void resolverMunicipioAction(despacho.municipioId).then(async (resultado) => {
      if (!activo) {
        return;
      }
      if (!resultado.ok) {
        setResolviendoMunicipio(false);
        toast.error(resultado.message);
        return;
      }

      setMunicipioActual(resultado.municipio);
      setDepartamentoId(resultado.departamentoId);

      const listado = await obtenerMunicipiosAction(resultado.departamentoId);
      if (!activo) {
        return;
      }
      setResolviendoMunicipio(false);
      if (listado.ok) {
        setMunicipios(listado.municipios);
      } else {
        toast.error(listado.message);
      }
    });

    return () => {
      activo = false;
    };
  }, [open, despacho]);

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

  // Carga los municipios cuando cambia el departamento seleccionado.
  async function cambiarDepartamento(valor: string) {
    setDepartamentoId(valor);
    setMunicipioId("");
    setMunicipioActual(null);
    setMunicipios([]);

    if (!valor) {
      return;
    }

    setCargandoMunicipios(true);
    const resultado = await obtenerMunicipiosAction(valor);
    setCargandoMunicipios(false);

    if (resultado.ok) {
      setMunicipios(resultado.municipios);
    } else {
      toast.error(resultado.message);
    }
  }

  const distritosFiltrados = catalogos.distritos.filter(
    (distrito) => !jurisdiccionId || distrito.jurisdiccionId === jurisdiccionId,
  );
  const circuitosFiltrados = catalogos.circuitos.filter(
    (circuito) => !distritoId || circuito.distritoId === distritoId,
  );
  const subEspecialidadesFiltradas = catalogos.subEspecialidades.filter(
    (subEspecialidad) =>
      !especialidadId || subEspecialidad.especialidadId === especialidadId,
  );
  const entidadesFiltradas = catalogos.entidades.filter(
    (entidad) => !jurisdiccionId || entidad.jurisdiccionId === jurisdiccionId,
  );

  const opcionesMunicipio: { id: string; nombre: string }[] = municipios.map((municipio) => ({
    id: municipio.id,
    nombre: municipio.nombre,
  }));
  if (despacho && !opcionesMunicipio.some((municipio) => municipio.id === despacho.municipioId)) {
    opcionesMunicipio.unshift({
      id: despacho.municipioId,
      nombre: municipioActual?.nombre ?? "Municipio actual",
    });
  }

  const titulo = esEdicion ? "Editar despacho" : "Nuevo despacho";
  const descripcion = esEdicion
    ? `Actualiza los datos de ${despacho.nombre}.`
    : "Registra un despacho judicial con su ubicación y clasificación.";

  return (
    <Dialog
      open={open}
      onOpenChange={(nuevoEstado) => {
        if (!isPending) {
          onOpenChange(nuevoEstado);
        }
      }}
    >
      <DialogContent className="sm:max-w-3xl">
        <DialogHeader>
          <span className="mb-1 flex h-10 w-10 items-center justify-center rounded-xl bg-corporativo-amarillo/30 text-corporativo-azul">
            {esEdicion ? <Pencil className="h-5 w-5" /> : <Building2 className="h-5 w-5" />}
          </span>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>{descripcion}</DialogDescription>
        </DialogHeader>

        <form action={accion} className="space-y-6">
          {despacho && <input type="hidden" name="id" value={despacho.id} />}

          <FormSection titulo="Datos básicos">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Código" htmlFor="despacho-codigo" required>
                <Input
                  id="despacho-codigo"
                  name="codigo"
                  defaultValue={despacho?.codigo ?? ""}
                  maxLength={50}
                  required
                  autoFocus
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Nombre" htmlFor="despacho-nombre" required>
                <Input
                  id="despacho-nombre"
                  name="nombre"
                  defaultValue={despacho?.nombre ?? ""}
                  maxLength={200}
                  required
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Estado" htmlFor="despacho-estado" required>
                <SelectField
                  id="despacho-estado"
                  name="estado"
                  defaultValue={despacho?.estado ?? "CREADO"}
                  required
                  disabled={isPending}
                >
                  {ESTADOS_DESPACHO.map((opcion) => (
                    <option key={opcion.value} value={opcion.value}>
                      {opcion.label}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <div className="flex items-end">
                <label className="flex w-full items-center gap-3 rounded-xl border border-slate-200 bg-white px-3 py-3">
                  <Checkbox
                    name="promiscuo"
                    defaultChecked={despacho?.promiscuo ?? false}
                    disabled={isPending}
                  />
                  <span className="text-sm font-medium text-slate-800">
                    Despacho promiscuo
                  </span>
                </label>
              </div>
            </div>
          </FormSection>

          <FormSection titulo="Ubicación">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Departamento" htmlFor="despacho-departamento">
                <SelectField
                  id="despacho-departamento"
                  value={departamentoId}
                  onChange={(event) => void cambiarDepartamento(event.target.value)}
                  disabled={isPending || resolviendoMunicipio}
                >
                  <option value="">Selecciona un departamento</option>
                  {catalogos.departamentos.map((departamento) => (
                    <option key={departamento.id} value={departamento.id}>
                      {departamento.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Municipio" htmlFor="despacho-municipio" required>
                <SelectField
                  id="despacho-municipio"
                  name="municipioId"
                  value={municipioId}
                  onChange={(event) => setMunicipioId(event.target.value)}
                  required
                  disabled={
                    isPending ||
                    !departamentoId ||
                    cargandoMunicipios ||
                    resolviendoMunicipio
                  }
                >
                  <option value="">
                    {cargandoMunicipios
                      ? "Cargando municipios…"
                      : "Selecciona un municipio"}
                  </option>
                  {opcionesMunicipio.map((municipio) => (
                    <option key={municipio.id} value={municipio.id}>
                      {municipio.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Jurisdicción" htmlFor="despacho-jurisdiccion" required>
                <SelectField
                  id="despacho-jurisdiccion"
                  name="jurisdiccionId"
                  value={jurisdiccionId}
                  onChange={(event) => {
                    setJurisdiccionId(event.target.value);
                    setDistritoId("");
                    setCircuitoId("");
                    setEntidadId("");
                  }}
                  required
                  disabled={isPending}
                >
                  <option value="">Selecciona una jurisdicción</option>
                  {catalogos.jurisdicciones.map((jurisdiccion) => (
                    <option key={jurisdiccion.id} value={jurisdiccion.id}>
                      {jurisdiccion.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Distrito" htmlFor="despacho-distrito">
                <SelectField
                  id="despacho-distrito"
                  name="distritoId"
                  value={distritoId}
                  onChange={(event) => {
                    setDistritoId(event.target.value);
                    setCircuitoId("");
                  }}
                  disabled={isPending || !jurisdiccionId}
                >
                  <option value="">Sin distrito</option>
                  {distritosFiltrados.map((distrito) => (
                    <option key={distrito.id} value={distrito.id}>
                      {distrito.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Circuito" htmlFor="despacho-circuito">
                <SelectField
                  id="despacho-circuito"
                  name="circuitoId"
                  value={circuitoId}
                  onChange={(event) => setCircuitoId(event.target.value)}
                  disabled={isPending || !distritoId}
                >
                  <option value="">Sin circuito</option>
                  {circuitosFiltrados.map((circuito) => (
                    <option key={circuito.id} value={circuito.id}>
                      {circuito.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
            </div>
          </FormSection>

          <FormSection titulo="Clasificación">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Especialidad" htmlFor="despacho-especialidad">
                <SelectField
                  id="despacho-especialidad"
                  name="especialidadId"
                  value={especialidadId}
                  onChange={(event) => {
                    setEspecialidadId(event.target.value);
                    setSubEspecialidadId("");
                  }}
                  disabled={isPending}
                >
                  <option value="">Sin especialidad</option>
                  {catalogos.especialidades.map((especialidad) => (
                    <option key={especialidad.id} value={especialidad.id}>
                      {especialidad.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Sub especialidad" htmlFor="despacho-subEspecialidad">
                <SelectField
                  id="despacho-subEspecialidad"
                  name="subEspecialidadId"
                  value={subEspecialidadId}
                  onChange={(event) => setSubEspecialidadId(event.target.value)}
                  disabled={isPending || !especialidadId}
                >
                  <option value="">Sin sub especialidad</option>
                  {subEspecialidadesFiltradas.map((subEspecialidad) => (
                    <option key={subEspecialidad.id} value={subEspecialidad.id}>
                      {subEspecialidad.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Entidad" htmlFor="despacho-entidad">
                <SelectField
                  id="despacho-entidad"
                  name="entidadId"
                  value={entidadId}
                  onChange={(event) => setEntidadId(event.target.value)}
                  disabled={isPending}
                >
                  <option value="">Sin entidad</option>
                  {entidadesFiltradas.map((entidad) => (
                    <option key={entidad.id} value={entidad.id}>
                      {entidad.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field
                label="Distrito de la sub especialidad"
                htmlFor="despacho-distritoSubEspecialidad"
              >
                <SelectField
                  id="despacho-distritoSubEspecialidad"
                  name="distritoSubEspecialidadId"
                  defaultValue={despacho?.distritoSubEspecialidadId ?? ""}
                  disabled={isPending}
                >
                  <option value="">Sin distrito</option>
                  {catalogos.distritos.map((distrito) => (
                    <option key={distrito.id} value={distrito.id}>
                      {distrito.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field
                label="Circuito de la sub especialidad"
                htmlFor="despacho-circuitoSubEspecialidad"
              >
                <SelectField
                  id="despacho-circuitoSubEspecialidad"
                  name="circuitoSubEspecialidadId"
                  defaultValue={despacho?.circuitoSubEspecialidadId ?? ""}
                  disabled={isPending}
                >
                  <option value="">Sin circuito</option>
                  {catalogos.circuitos.map((circuito) => (
                    <option key={circuito.id} value={circuito.id}>
                      {circuito.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
            </div>
          </FormSection>

          <FormSection titulo="Contacto">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Dirección" htmlFor="despacho-direccion">
                <Input
                  id="despacho-direccion"
                  name="direccion"
                  defaultValue={despacho?.direccion ?? ""}
                  maxLength={255}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Complemento de dirección" htmlFor="despacho-complemento">
                <Input
                  id="despacho-complemento"
                  name="complementoDireccion"
                  defaultValue={despacho?.complementoDireccion ?? ""}
                  maxLength={150}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Teléfono fijo" htmlFor="despacho-telefonoFijo">
                <Input
                  id="despacho-telefonoFijo"
                  name="telefonoFijo"
                  defaultValue={despacho?.telefonoFijo ?? ""}
                  maxLength={50}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Teléfono celular" htmlFor="despacho-telefonoCelular">
                <Input
                  id="despacho-telefonoCelular"
                  name="telefonoCelular"
                  defaultValue={despacho?.telefonoCelular ?? ""}
                  maxLength={50}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Correo electrónico" htmlFor="despacho-correo">
                <Input
                  id="despacho-correo"
                  name="correo"
                  type="email"
                  defaultValue={despacho?.correo ?? ""}
                  maxLength={150}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
              <Field label="Horario de atención" htmlFor="despacho-horario">
                <Input
                  id="despacho-horario"
                  name="horarioAtencion"
                  defaultValue={despacho?.horarioAtencion ?? ""}
                  maxLength={150}
                  disabled={isPending}
                  className="h-11"
                />
              </Field>
            </div>
          </FormSection>

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
              disabled={isPending || resolviendoMunicipio}
              className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
            >
              {isPending ? <LoaderCircle className="animate-spin" /> : <Save />}
              {isPending ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear despacho"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

/** Sección visual del formulario con su título. */
function FormSection({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <section className="space-y-3">
      <h3 className="border-b border-slate-100 pb-2 text-sm font-semibold text-corporativo-azul">
        {titulo}
      </h3>
      {children}
    </section>
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

/** Selector nativo estilizado para mantener la coherencia visual sin nuevas dependencias. */
function SelectField({
  className,
  children,
  ...props
}: ComponentProps<"select">) {
  return (
    <select
      className={cn(
        "h-11 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-800 outline-none transition-colors focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-60",
        className,
      )}
      {...props}
    >
      {children}
    </select>
  );
}
