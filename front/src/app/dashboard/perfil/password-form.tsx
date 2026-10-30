"use client";

import { useActionState, useEffect, useRef, useState } from "react";
import { motion, useReducedMotion } from "motion/react";
import { KeyRound, LoaderCircle } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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
import { getCambiarContrasenaValidationError } from "@/lib/perfil/perfil-validation";
import { cambiarContrasenaAction, type PerfilActionState } from "./actions";

const ESTADO_INICIAL: PerfilActionState = { ok: false, message: null };

/**
 * Formulario de cambio de contraseña. Valida antes de pedir confirmación en un AlertDialog;
 * al confirmar envía el formulario. La sesión actual sigue activa.
 */
export function PasswordForm() {
  const shouldReduceMotion = useReducedMotion();
  const formRef = useRef<HTMLFormElement>(null);
  const [isConfirmOpen, setIsConfirmOpen] = useState(false);
  const [estado, accion, isPending] = useActionState(cambiarContrasenaAction, ESTADO_INICIAL);

  // Notifica el resultado y limpia el formulario tras un cambio exitoso.
  useEffect(() => {
    if (!estado.message) {
      return;
    }
    if (estado.ok) {
      toast.success(estado.message);
      formRef.current?.reset();
    } else {
      toast.error(estado.message);
    }
  }, [estado]);

  /** Valida los campos y abre la confirmación; no envía nada todavía. */
  function solicitarConfirmacion() {
    const form = formRef.current;
    if (!form) {
      return;
    }
    const datos = new FormData(form);
    const validationError = getCambiarContrasenaValidationError({
      contrasenaActual: String(datos.get("contrasenaActual") ?? ""),
      contrasenaNueva: String(datos.get("contrasenaNueva") ?? ""),
      confirmacion: String(datos.get("confirmacion") ?? ""),
    });
    if (validationError) {
      toast.error(validationError);
      return;
    }
    setIsConfirmOpen(true);
  }

  /** Cierra el diálogo y después envía el formulario, para no dejar bloqueada la página. */
  function confirmarCambio() {
    setIsConfirmOpen(false);
    formRef.current?.requestSubmit();
  }

  return (
    <motion.section
      initial={shouldReduceMotion ? false : { opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.45, delay: 0.1, ease: "easeOut" }}
      className="rounded-2xl border border-corporativo-azul/10 bg-white p-6 shadow-sm"
    >
      <header className="mb-6 flex items-start gap-3">
        <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-corporativo-amarillo/30 text-corporativo-azul">
          <KeyRound className="h-5 w-5" />
        </span>
        <div className="space-y-1">
          <h2 className="text-lg font-semibold text-corporativo-azul">Cambiar contraseña</h2>
          <p className="text-sm text-slate-500">La nueva contraseña debe tener entre 8 y 100 caracteres.</p>
        </div>
      </header>

      <form ref={formRef} action={accion} className="space-y-5">
        <div className="space-y-2">
          <Label htmlFor="contrasenaActual">Contraseña actual</Label>
          <Input
            id="contrasenaActual"
            name="contrasenaActual"
            type="password"
            autoComplete="current-password"
            maxLength={100}
            required
            disabled={isPending}
            className="h-11"
          />
        </div>
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="space-y-2">
            <Label htmlFor="contrasenaNueva">Nueva contraseña</Label>
            <Input
              id="contrasenaNueva"
              name="contrasenaNueva"
              type="password"
              autoComplete="new-password"
              minLength={8}
              maxLength={100}
              required
              disabled={isPending}
              className="h-11"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="confirmacion">Confirmar nueva contraseña</Label>
            <Input
              id="confirmacion"
              name="confirmacion"
              type="password"
              autoComplete="new-password"
              minLength={8}
              maxLength={100}
              required
              disabled={isPending}
              className="h-11"
            />
          </div>
        </div>

        <div className="flex justify-end">
          <Button
            type="button"
            onClick={solicitarConfirmacion}
            disabled={isPending}
            className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
          >
            {isPending && <LoaderCircle className="animate-spin" />}
            {isPending ? "Cambiando..." : "Cambiar contraseña"}
          </Button>
        </div>
      </form>

      <AlertDialog open={isConfirmOpen} onOpenChange={setIsConfirmOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>¿Cambiar tu contraseña?</AlertDialogTitle>
            <AlertDialogDescription>
              Tu sesión se cerrará al cambiarla. Inicia sesión de nuevo con la contraseña nueva.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancelar</AlertDialogCancel>
            <AlertDialogAction onClick={confirmarCambio}>Sí, cambiar</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </motion.section>
  );
}
