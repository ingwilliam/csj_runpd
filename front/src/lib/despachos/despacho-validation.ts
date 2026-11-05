import type { DespachoDatos } from "@/lib/backend/types";

const MAX_CODIGO = 50;
const MAX_NOMBRE = 200;
const MAX_DIRECCION = 255;
const MAX_COMPLEMENTO = 150;
const MAX_TELEFONO = 50;
const MAX_CORREO = 150;
const MAX_HORARIO = 150;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const GUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
const ESTADOS_VALIDOS = ["CREADO", "MODIFICADO", "TRASLADADO", "SUPRIMIDO"];

/**
 * Valida los datos de un despacho con las mismas reglas del backend.
 * @param datos Datos del despacho a crear o actualizar.
 * @returns Mensaje de error en español, o null si los datos son válidos.
 */
export function getDespachoValidationError(datos: DespachoDatos): string | null {
  // Paso 1: campos obligatorios y longitudes.
  if (!datos.codigo.trim()) {
    return "El código es obligatorio.";
  }
  if (datos.codigo.trim().length > MAX_CODIGO) {
    return `El código no puede superar ${MAX_CODIGO} caracteres.`;
  }
  if (!datos.nombre.trim()) {
    return "El nombre es obligatorio.";
  }
  if (datos.nombre.trim().length > MAX_NOMBRE) {
    return `El nombre no puede superar ${MAX_NOMBRE} caracteres.`;
  }
  if (!ESTADOS_VALIDOS.includes(datos.estado)) {
    return "El estado debe ser Creado, Modificado, Trasladado o Suprimido.";
  }

  const textos = [
    { valor: datos.direccion, limite: MAX_DIRECCION, etiqueta: "La dirección" },
    { valor: datos.complementoDireccion, limite: MAX_COMPLEMENTO, etiqueta: "El complemento de dirección" },
    { valor: datos.telefonoFijo, limite: MAX_TELEFONO, etiqueta: "El teléfono fijo" },
    { valor: datos.telefonoCelular, limite: MAX_TELEFONO, etiqueta: "El teléfono celular" },
    { valor: datos.correo, limite: MAX_CORREO, etiqueta: "El correo" },
    { valor: datos.horarioAtencion, limite: MAX_HORARIO, etiqueta: "El horario de atención" },
  ];

  for (const texto of textos) {
    if (texto.valor && texto.valor.length > texto.limite) {
      return `${texto.etiqueta} no puede superar ${texto.limite} caracteres.`;
    }
  }

  if (datos.correo && !EMAIL_PATTERN.test(datos.correo)) {
    return "El correo electrónico no tiene un formato válido.";
  }

  // Paso 2: relaciones obligatorias.
  if (!esIdentificadorValido(datos.municipioId)) {
    return "Debes seleccionar un municipio.";
  }
  if (!esIdentificadorValido(datos.jurisdiccionId)) {
    return "Debes seleccionar una jurisdicción.";
  }

  // Paso 3: relaciones opcionales.
  const relacionesOpcionales = [
    { valor: datos.especialidadId, etiqueta: "La especialidad" },
    { valor: datos.subEspecialidadId, etiqueta: "La sub especialidad" },
    { valor: datos.distritoId, etiqueta: "El distrito" },
    { valor: datos.circuitoId, etiqueta: "El circuito" },
    { valor: datos.distritoSubEspecialidadId, etiqueta: "El distrito de la sub especialidad" },
    { valor: datos.circuitoSubEspecialidadId, etiqueta: "El circuito de la sub especialidad" },
    { valor: datos.entidadId, etiqueta: "La entidad" },
  ];

  for (const relacion of relacionesOpcionales) {
    if (relacion.valor && !esIdentificadorValido(relacion.valor)) {
      return `${relacion.etiqueta} seleccionada no es válida.`;
    }
  }

  return null;
}

/**
 * Indica si un texto tiene forma de GUID, como los identificadores que expone el backend.
 * @param valor Texto a comprobar.
 * @returns true si el formato es válido.
 */
export function esIdentificadorValido(valor: string): boolean {
  return GUID_PATTERN.test(valor);
}
