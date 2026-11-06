"use client";

import {
  useActionState,
  useCallback,
  useEffect,
  useRef,
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
import type {
  CatalogosDespachos,
  Despacho,
  Municipio,
  ResolverTerritorioResponse,
} from "@/lib/backend/types";
import { ESTADOS_DESPACHO } from "@/lib/despachos/despacho-formato";
import { esIdentificadorValido } from "@/lib/despachos/despacho-validation";
import {
  actualizarDespachoAction,
  crearDespachoAction,
  obtenerMunicipiosAction,
  resolverMunicipioAction,
  resolverTerritorioAction,
  type DespachoActionState,
} from "./actions";
import { DespachoTerritorioCard } from "./despacho-territorio-card";

const ESTADO_INICIAL: DespachoActionState = { ok: false, message: null };

/** Identificador fijo de la jurisdicción ordinaria en el catálogo maestro. */
const JURISDICCION_ORDINARIA_ID = "1c9b35fc-c46b-4ff1-bb35-f4eb23f8a5ba";

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
  const [promiscuo, setPromiscuo] = useState(despacho?.promiscuo ?? false);
  const [distritoId, setDistritoId] = useState(despacho?.distritoId ?? "");
  const [circuitoId, setCircuitoId] = useState(despacho?.circuitoId ?? "");
  const [especialidadId, setEspecialidadId] = useState(despacho?.especialidadId ?? "");
  const [subEspecialidadId, setSubEspecialidadId] = useState(
    despacho?.subEspecialidadId ?? "",
  );
  const [entidadId, setEntidadId] = useState(despacho?.entidadId ?? "");
  const [distritoSubEspecialidadId, setDistritoSubEspecialidadId] = useState(
    despacho?.distritoSubEspecialidadId ?? "",
  );
  const [circuitoSubEspecialidadId, setCircuitoSubEspecialidadId] = useState(
    despacho?.circuitoSubEspecialidadId ?? "",
  );
  const [territorio, setTerritorio] = useState<ResolverTerritorioResponse | null>(null);
  const tieneDatosTerritorialesIniciales = despacho
    ? esIdentificadorValido(despacho.jurisdiccionId) &&
      esIdentificadorValido(despacho.municipioId)
    : false;
  const [cargandoTerritorio, setCargandoTerritorio] = useState(tieneDatosTerritorialesIniciales);
  const [errorTerritorio, setErrorTerritorio] = useState<string | null>(null);
  const [consecutivo, setConsecutivo] = useState(despacho?.codigo?.slice(-3) ?? "");
  const [nombre, setNombre] = useState(despacho?.nombre ?? "");
  const [municipioSeleccionado, setMunicipioSeleccionado] = useState<Municipio | null>(
    despacho ? municipioActual : null,
  );
  const consultaTerritorioRef = useRef(0);

  const aplicarTerritorio = useCallback(
    (
      resultado: ResolverTerritorioResponse,
      subEspecialidadConsultada: string,
    ) => {
      setTerritorio(resultado);
      const { general, especifico } = resultado;
      if (general) {
        setDistritoId(general.distritoId);
        setCircuitoId(general.circuitoId ?? "");
      }
      if (especifico) {
        setDistritoSubEspecialidadId(especifico.distritoId);
        setCircuitoSubEspecialidadId(especifico.circuitoId ?? "");
      } else if (!subEspecialidadConsultada) {
        setDistritoSubEspecialidadId("");
        setCircuitoSubEspecialidadId("");
      }
    },
    [],
  );

  async function consultarTerritorio(
    jurisdiccion: string,
    municipio: string,
    subespecialidad: string,
  ) {
    const consultaId = ++consultaTerritorioRef.current;
    setErrorTerritorio(null);

    if (!esIdentificadorValido(jurisdiccion) || !esIdentificadorValido(municipio)) {
      setTerritorio(null);
      setCargandoTerritorio(false);
      return;
    }

    setCargandoTerritorio(true);
    const resultado = await resolverTerritorioAction({
      jurisdiccionId: jurisdiccion,
      municipioId: municipio,
      subEspecialidadId: subespecialidad || null,
    });

    if (consultaId !== consultaTerritorioRef.current) {
      return;
    }

    setCargandoTerritorio(false);
    if (!resultado.ok) {
      setErrorTerritorio(resultado.message);
      return;
    }

    aplicarTerritorio(resultado.territorio, subespecialidad);
  }

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
      setMunicipioSeleccionado(resultado.municipio);
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

  // Al editar, consulta la clasificación territorial de los valores actuales.
  useEffect(() => {
    if (!open || !despacho) {
      return;
    }

    if (
      !esIdentificadorValido(despacho.jurisdiccionId) ||
      !esIdentificadorValido(despacho.municipioId)
    ) {
      return;
    }

    const consultaId = ++consultaTerritorioRef.current;
    void resolverTerritorioAction({
      jurisdiccionId: despacho.jurisdiccionId,
      municipioId: despacho.municipioId,
      subEspecialidadId: despacho.subEspecialidadId ?? null,
    }).then((resultado) => {
      if (consultaId !== consultaTerritorioRef.current) {
        return;
      }

      setCargandoTerritorio(false);
      if (!resultado.ok) {
        setErrorTerritorio(resultado.message);
        return;
      }

      aplicarTerritorio(resultado.territorio, despacho.subEspecialidadId ?? "");
    });
  }, [open, despacho, aplicarTerritorio]);

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
    setMunicipioSeleccionado(null);
    setConsecutivo("");
    setMunicipios([]);
    setDistritoId("");
    setCircuitoId("");
    setDistritoSubEspecialidadId("");
    setCircuitoSubEspecialidadId("");
    setTerritorio(null);
    setErrorTerritorio(null);
    setCargandoTerritorio(false);
    consultaTerritorioRef.current += 1;

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
  const esJurisdiccionOrdinaria = jurisdiccionId === JURISDICCION_ORDINARIA_ID;
  const subEspecialidadesDisponibles = promiscuo
    ? catalogos.subEspecialidades.filter((subEspecialidad) => subEspecialidad.promiscuo)
    : catalogos.subEspecialidades.filter(
        (subEspecialidad) => subEspecialidad.especialidadId === especialidadId,
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

  // Código del despacho generado automáticamente a partir de los catálogos maestros.
  // Estructura: departamento(2) + municipio(3) + entidad(2) + subespecialidad(2) + consecutivo(3) = 12 dígitos.
  const departamento = catalogos.departamentos.find(
    (departamento) => departamento.id === departamentoId,
  );
  const entidad = catalogos.entidades.find((entidad) => entidad.id === entidadId);
  const subEspecialidad = catalogos.subEspecialidades.find(
    (subEspecialidad) => subEspecialidad.id === subEspecialidadId,
  );

  const parteDepartamento = (departamento?.codigoDivpola ?? "").padStart(2, "0") || "00";
  const codigoMunicipio = municipioSeleccionado?.codigoDivpola ?? "";
  const parteMunicipio = codigoMunicipio.length >= 3
    ? codigoMunicipio.slice(-3)
    : codigoMunicipio.padStart(3, "0") || "000";
  const parteEntidad = (entidad?.codigo ?? "").padStart(2, "0") || "00";
  const parteSubEspecialidad = (subEspecialidad?.codigo ?? "").padStart(2, "0") || "00";
  const parteConsecutivo = consecutivo.trim() !== "" ? consecutivo.padStart(3, "0") : "000";

  const codigoCompleto =
    `${parteDepartamento}${parteMunicipio}${parteEntidad}${parteSubEspecialidad}${parteConsecutivo}`;
  const codigoValido = /^\d{3}$/.test(consecutivo);

  return (
    <Dialog
      open={open}
      onOpenChange={(nuevoEstado) => {
        if (!isPending) {
          onOpenChange(nuevoEstado);
        }
      }}
    >
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <span className="mb-1 flex h-10 w-10 items-center justify-center rounded-xl bg-corporativo-amarillo/30 text-corporativo-azul">
            {esEdicion ? <Pencil className="h-5 w-5" /> : <Building2 className="h-5 w-5" />}
          </span>
          <DialogTitle>{titulo}</DialogTitle>
          <DialogDescription>{descripcion}</DialogDescription>
        </DialogHeader>

        <form action={accion} className="space-y-6">
          {despacho && <input type="hidden" name="id" value={despacho.id} />}
          <input type="hidden" name="codigo" value={codigoCompleto} />

          <FormSection titulo="Datos básicos" className="hidden">
            <div className="grid gap-4 sm:grid-cols-2">
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
            </div>
          </FormSection>

          <FormSection titulo="Ubicación y tipo de despacho">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Jurisdicción" htmlFor="despacho-jurisdiccion" required>
                <SelectField
                  id="despacho-jurisdiccion"
                  name="jurisdiccionId"
                  value={jurisdiccionId}
                  onChange={(event) => {
                    const nuevaJurisdiccionId = event.target.value;
                    const nuevaSubEspecialidadId =
                      nuevaJurisdiccionId === JURISDICCION_ORDINARIA_ID ? subEspecialidadId : "";
                    setJurisdiccionId(nuevaJurisdiccionId);
                    setDistritoId("");
                    setCircuitoId("");
                    setEntidadId("");
                    // Promiscuo, especialidad y subespecialidad solo aplican a la jurisdicción ordinaria.
                    if (nuevaJurisdiccionId !== JURISDICCION_ORDINARIA_ID) {
                      setPromiscuo(false);
                      setEspecialidadId("");
                      setSubEspecialidadId("");
                      setDistritoSubEspecialidadId("");
                      setCircuitoSubEspecialidadId("");
                    }
                    void consultarTerritorio(
                      nuevaJurisdiccionId,
                      municipioId,
                      nuevaSubEspecialidadId,
                    );
                  }}
                  required
                  disabled={isPending}
                  autoFocus
                >
                  <option value="">Selecciona una jurisdicción</option>
                  {catalogos.jurisdicciones.map((jurisdiccion) => (
                    <option key={jurisdiccion.id} value={jurisdiccion.id}>
                      {jurisdiccion.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
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
              <Field label="Municipio / Ciudad" htmlFor="despacho-municipio" required>
                <SelectField
                  id="despacho-municipio"
                  name="municipioId"
                  value={municipioId}
                  onChange={(event) => {
                    const nuevoMunicipioId = event.target.value;
                    setMunicipioId(nuevoMunicipioId);
                    setMunicipioSeleccionado(
                      municipios.find((m) => m.id === nuevoMunicipioId) ?? null,
                    );
                    setDistritoId("");
                    setCircuitoId("");
                    setDistritoSubEspecialidadId("");
                    setCircuitoSubEspecialidadId("");
                    void consultarTerritorio(
                      jurisdiccionId,
                      nuevoMunicipioId,
                      subEspecialidadId,
                    );
                  }}
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
                      : "Selecciona un municipio o ciudad"}
                  </option>
                  {opcionesMunicipio.map((municipio) => (
                    <option key={municipio.id} value={municipio.id}>
                      {municipio.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field
                label="Tipo de despacho"
                htmlFor="despacho-entidad"
                hint={
                  jurisdiccionId
                    ? undefined
                    : "Selecciona una jurisdicción para habilitar este campo."
                }
              >
                <SelectField
                  id="despacho-entidad"
                  name="entidadId"
                  value={entidadId}
                  onChange={(event) => setEntidadId(event.target.value)}
                  disabled={isPending || !jurisdiccionId}
                >
                  <option value="">Selecciona un tipo de despacho</option>
                  {entidadesFiltradas.map((entidad) => (
                    <option key={entidad.id} value={entidad.id}>
                      {entidad.nombre}
                    </option>
                  ))}
                </SelectField>
              </Field>
              <Field label="Distrito" htmlFor="despacho-distrito" className="hidden">
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
              <Field label="Circuito" htmlFor="despacho-circuito" className="hidden">
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

          {esJurisdiccionOrdinaria && (
            <FormSection titulo="Clasificación">
              <div className="grid gap-4 sm:grid-cols-2">
                <div className="flex items-end sm:col-span-2">
                  <label className="flex w-full items-center gap-3 rounded-xl border border-slate-200 bg-white px-3 py-3">
                    <Checkbox
                      name="promiscuo"
                      checked={promiscuo}
                      onCheckedChange={(valor) => {
                        const activo = valor === true;
                        setPromiscuo(activo);
                        setSubEspecialidadId("");
                        setDistritoSubEspecialidadId("");
                        setCircuitoSubEspecialidadId("");
                        if (activo) {
                          setEspecialidadId("");
                        }
                        void consultarTerritorio(jurisdiccionId, municipioId, "");
                      }}
                      disabled={isPending}
                    />
                    <span className="text-sm font-medium text-slate-800">
                      ¿Es un Juzgado Promiscuo?
                    </span>
                  </label>
                </div>
                {!promiscuo && (
                  <Field label="Especialidad" htmlFor="despacho-especialidad">
                    <SelectField
                      id="despacho-especialidad"
                      name="especialidadId"
                      value={especialidadId}
                      onChange={(event) => {
                        const nuevaEspecialidadId = event.target.value;
                        setEspecialidadId(nuevaEspecialidadId);
                        setSubEspecialidadId("");
                        setDistritoSubEspecialidadId("");
                        setCircuitoSubEspecialidadId("");
                        void consultarTerritorio(jurisdiccionId, municipioId, "");
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
                )}
                <Field label="Sub especialidad" htmlFor="despacho-subEspecialidad">
                  <SelectField
                    id="despacho-subEspecialidad"
                    name="subEspecialidadId"
                    value={subEspecialidadId}
                    onChange={(event) => {
                      const nuevaSubEspecialidadId = event.target.value;
                      setSubEspecialidadId(nuevaSubEspecialidadId);
                      setDistritoSubEspecialidadId("");
                      setCircuitoSubEspecialidadId("");
                      void consultarTerritorio(
                        jurisdiccionId,
                        municipioId,
                        nuevaSubEspecialidadId,
                      );
                    }}
                    disabled={isPending || (!promiscuo && !especialidadId)}
                  >
                    <option value="">Sin sub especialidad</option>
                    {subEspecialidadesDisponibles.map((subEspecialidad) => (
                      <option key={subEspecialidad.id} value={subEspecialidad.id}>
                        {subEspecialidad.nombre}
                      </option>
                    ))}
                  </SelectField>
                </Field>
              </div>
            </FormSection>
          )}

          <DespachoTerritorioCard
            catalogos={catalogos}
            cargando={cargandoTerritorio}
            error={errorTerritorio}
            territorio={territorio}
            jurisdiccionId={jurisdiccionId}
            municipioId={municipioId}
            subEspecialidadId={subEspecialidadId}
          />

          <FormSection titulo="Clasificación" className="hidden">
            <div className="grid gap-4 sm:grid-cols-2">
              <Field
                label="Distrito de la sub especialidad"
                htmlFor="despacho-distritoSubEspecialidad"
              >
                <SelectField
                  id="despacho-distritoSubEspecialidad"
                  name="distritoSubEspecialidadId"
                  value={distritoSubEspecialidadId}
                  onChange={(event) => setDistritoSubEspecialidadId(event.target.value)}
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
                  value={circuitoSubEspecialidadId}
                  onChange={(event) => setCircuitoSubEspecialidadId(event.target.value)}
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

          <FormSection titulo="Contacto" className="hidden">
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

          <section className="space-y-4">
            <div className="rounded-2xl border border-corporativo-azul/20 bg-corporativo-amarillo/10 p-4">
              <h3 className="flex items-center justify-center gap-2 text-sm font-bold text-corporativo-azul uppercase">
                <span className="text-corporativo-amarillo">🏛️</span>
                CÓDIGO ESTRUCTURAL CONFIGURADO
              </h3>
              <p className="flex justify-center text-[10px] font-semibold tracking-wider text-slate-500 uppercase mt-1">
                Automático
              </p>
              <div className="flex items-center justify-center gap-2 flex-wrap">
                <Segmento valor={parteDepartamento} etiqueta="DEPTO (2)" />
                <span className="text-corporativo-amarillo/40 font-bold">-</span>
                <Segmento valor={parteMunicipio} etiqueta="MUNI (3)" />
                <span className="text-corporativo-amarillo/40 font-bold">-</span>
                <Segmento valor={parteEntidad} etiqueta="ENT (2)" />
                <span className="text-corporativo-amarillo/40 font-bold">-</span>
                <Segmento valor={parteSubEspecialidad} etiqueta="ESP (2)" />
                <span className="text-corporativo-amarillo/40 font-bold">-</span>
                <Input
                  id="despacho-consecutivo"
                  value={consecutivo}
                  onChange={(event) => {
                    setConsecutivo(event.target.value.replace(/\D/g, "").slice(0, 3));
                  }}
                  maxLength={3}
                  disabled={isPending || !municipioSeleccionado}
                  className="h-9 w-16 rounded border border-corporativo-azul bg-white px-2 py-1 text-xs font-mono font-bold text-corporativo-azul outline-none focus-visible:border-corporativo-azul focus-visible:ring-1 focus-visible:ring-corporativo-azul/20 disabled:cursor-not-allowed disabled:opacity-60"
                />
                <span className="text-[10px] text-corporativo-amarillo/60 font-bold uppercase">
                  CONSECUTIVO (3)
                </span>
              </div>
            </div>

            <Field label="Nombre del despacho" htmlFor="despacho-nombre" required>
              <Input
                id="despacho-nombre"
                name="nombre"
                value={nombre}
                onChange={(event) => setNombre(event.target.value)}
                maxLength={200}
                disabled={isPending || !codigoValido}
                className="h-11"
              />
            </Field>

            {!codigoValido && municipioSeleccionado && (
              <p className="text-xs text-red-600 flex justify-center">
                Completa el consecutivo (3 dígitos) para habilitar el nombre y guardar.
              </p>
            )}
          </section>

          <DialogFooter>
            <DialogClose asChild>
              <Button type="button" variant="outline" disabled={isPending} className="h-11">
                Cancelar
              </Button>
            </DialogClose>
            <Button
              type="submit"
              disabled={isPending || resolviendoMunicipio || !codigoValido}
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
function FormSection({
  titulo,
  className,
  children,
}: {
  titulo: string;
  className?: string;
  children: ReactNode;
}) {
  return (
    <section className={cn("space-y-3", className)}>
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
  className,
  children,
}: {
  label: string;
  htmlFor: string;
  required?: boolean;
  hint?: string;
  className?: string;
  children: ReactNode;
}) {
  return (
    <div className={cn("space-y-2", className)}>
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

/** Muestra un segmento del código estructural con su valor y etiqueta. */
function Segmento({ valor, etiqueta }: { valor: string; etiqueta: string }) {
  return (
    <div className="flex flex-col items-center gap-0.5 rounded border border-slate-200 bg-white px-2 py-1.5 text-xs font-mono text-slate-800">
      <span>{valor}</span>
      <span className="text-slate-400">{etiqueta}</span>
    </div>
  );
}
