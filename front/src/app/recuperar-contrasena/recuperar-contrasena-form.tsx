"use client";

import { useState, type FormEvent } from "react";
import { CircleCheck, CircleAlert, LoaderCircle, Mail } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { getCorreoValidationError } from "@/lib/auth/login-validation";

const NETWORK_ERROR_MESSAGE = "No se pudo conectar con el servidor. Intenta de nuevo.";
const DEFAULT_ERROR_MESSAGE = "No se pudo procesar la solicitud. Intenta de nuevo.";

/**
 * Formulario para pedir una contraseña temporal. Muestra el mensaje genérico del backend,
 * que es el mismo exista o no el correo.
 */
export function RecuperarContrasenaForm() {
  const [correoElectronico, setCorreoElectronico] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [mensajeExito, setMensajeExito] = useState<string | null>(null);
  const [mensajeError, setMensajeError] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    // Paso 1: validar antes de llamar al servidor.
    const correo = correoElectronico.trim();
    const validationError = getCorreoValidationError(correo);
    if (validationError) {
      setMensajeExito(null);
      setMensajeError(validationError);
      return;
    }

    // Paso 2: pedir la contraseña temporal al Route Handler.
    setMensajeError(null);
    setMensajeExito(null);
    setIsLoading(true);
    try {
      const response = await fetch("/api/auth/recuperar-contrasena", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ correoElectronico: correo }),
      });
      const body = (await response.json().catch(() => null)) as { message?: string } | null;

      // Paso 3: mostrar el mensaje del servidor, o uno genérico si no es válido.
      if (!response.ok) {
        setMensajeError(body?.message ?? DEFAULT_ERROR_MESSAGE);
        return;
      }
      setMensajeExito(body?.message ?? "Revisa tu correo electrónico.");
    } catch {
      setMensajeError(NETWORK_ERROR_MESSAGE);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate className="space-y-6">
      <h2 className="text-xl font-semibold text-slate-800">Recuperar contraseña</h2>
      <p className="text-sm text-slate-600">
        Escribe tu correo electrónico. Si la cuenta existe, te enviaremos una contraseña temporal.
      </p>

      {mensajeError && (
        <div
          role="alert"
          className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
        >
          <CircleAlert className="mt-0.5 h-4 w-4 shrink-0" />
          <span>{mensajeError}</span>
        </div>
      )}

      {mensajeExito && (
        <div
          role="status"
          className="flex items-start gap-3 rounded-lg border border-corporativo-verde/30 bg-corporativo-verde/10 px-4 py-3 text-sm text-slate-800"
        >
          <CircleCheck className="mt-0.5 h-4 w-4 shrink-0 text-corporativo-verde" />
          <span>{mensajeExito}</span>
        </div>
      )}

      <div className="group space-y-2">
        <Label htmlFor="correoElectronico" className="text-slate-700">
          Correo electrónico
        </Label>
        <div className="relative">
          <Mail className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400 transition group-focus-within:text-corporativo-azul" />
          <Input
            id="correoElectronico"
            name="correoElectronico"
            type="email"
            autoComplete="username"
            placeholder="usuario@entidad.gov.co"
            value={correoElectronico}
            onChange={(event) => setCorreoElectronico(event.target.value)}
            disabled={isLoading}
            className="h-11 pl-10"
          />
        </div>
      </div>

      <Button
        type="submit"
        disabled={isLoading}
        className="h-11 w-full bg-corporativo-azul text-base text-white hover:bg-corporativo-azul/90"
      >
        {isLoading && <LoaderCircle className="h-5 w-5 animate-spin" />}
        {isLoading ? "Enviando..." : "Enviar contraseña temporal"}
      </Button>
    </form>
  );
}
