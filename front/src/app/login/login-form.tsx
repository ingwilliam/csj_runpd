"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useLayoutEffect, useState, type FormEvent } from "react";
import { AnimatePresence, motion, useAnimate, useReducedMotion, type Variants } from "motion/react";
import { CircleAlert, CircleCheck, Eye, EyeOff, LoaderCircle, Lock, Mail } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { getLoginValidationError } from "@/lib/auth/login-validation";
import type { LoginRequest } from "@/lib/backend/types";

const DEFAULT_ERROR_MESSAGE = "No se pudo iniciar sesión. Intenta de nuevo.";
const NETWORK_ERROR_MESSAGE = "No se pudo conectar con el servidor. Intenta de nuevo.";
// Tiempo que se muestra el estado de éxito antes de entrar al panel.
const SUCCESS_REDIRECT_DELAY_MS = 900;
const SHAKE_KEYFRAMES = [0, -8, 8, -6, 6, 0];
const DESTINO_CONTRASENA_TEMPORAL = "/dashboard/perfil?contrasenaTemporal=1";

const formVariants: Variants = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.08 } },
};

const fieldVariants: Variants = {
  hidden: { opacity: 0, y: 12 },
  visible: { opacity: 1, y: 0 },
};

/** Formulario de inicio de sesión: valida, llama al Route Handler y redirige al panel. */
export function LoginForm() {
  const router = useRouter();
  const shouldReduceMotion = useReducedMotion();
  const [scope, animate] = useAnimate<HTMLFormElement>();
  const [correoElectronico, setCorreoElectronico] = useState("");
  const [contrasena, setContrasena] = useState("");
  const [mostrarContrasena, setMostrarContrasena] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Next.js oculta esta página al navegar al panel en lugar de desmontarla, y el estado
  // (p. ej. "Acceso concedido") se conservaría al volver tras cerrar sesión.
  // Al ocultarse, se reinicia el estado de envío.
  useLayoutEffect(() => {
    return () => {
      setIsLoading(false);
      setIsSuccess(false);
      setErrorMessage(null);
    };
  }, []);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    // Paso 1: validar antes de llamar al servidor.
    const credentials: LoginRequest = {
      correoElectronico: correoElectronico.trim(),
      contrasena,
    };
    const validationError = getLoginValidationError(credentials);
    if (validationError) {
      showError(validationError);
      return;
    }

    // Paso 2: enviar credenciales al Route Handler del mismo servidor.
    setErrorMessage(null);
    setIsLoading(true);
    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(credentials),
      });

      // Paso 3: mostrar el error del servidor o celebrar y entrar al panel.
      if (!response.ok) {
        showError(await readErrorMessage(response));
        return;
      }
      const body = (await response.json()) as { debeCambiarContrasena?: boolean };
      setIsLoading(false);
      setIsSuccess(true);
      await wait(SUCCESS_REDIRECT_DELAY_MS);
      // Con contraseña temporal se lleva al perfil, donde se muestra el aviso para cambiarla.
      router.replace(body.debeCambiarContrasena ? DESTINO_CONTRASENA_TEMPORAL : "/dashboard");
    } catch {
      // Los fallos de red se notifican con toast porque no dependen de los campos.
      toast.error(NETWORK_ERROR_MESSAGE);
      setIsLoading(false);
      shakeForm();
    }
  }

  /** Muestra un error inline, libera el formulario y lo sacude para llamar la atención. */
  function showError(message: string) {
    setErrorMessage(message);
    setIsLoading(false);
    shakeForm();
  }

  /** Sacude el formulario; se omite si el usuario prefiere menos movimiento. */
  function shakeForm() {
    if (shouldReduceMotion || !scope.current) {
      return;
    }
    void animate(scope.current, { x: SHAKE_KEYFRAMES }, { duration: 0.4 });
  }

  const isBusy = isLoading || isSuccess;

  return (
    <motion.form
      ref={scope}
      onSubmit={handleSubmit}
      noValidate
      className="space-y-6"
      initial={shouldReduceMotion ? false : "hidden"}
      animate="visible"
      variants={formVariants}
    >
      <motion.h2 variants={fieldVariants} className="text-xl font-semibold text-slate-800">
        Iniciar sesión
      </motion.h2>

      <AnimatePresence>
        {errorMessage && (
          <motion.div
            key="login-error"
            role="alert"
            initial={{ opacity: 0, y: -8 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0 }}
            className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
          >
            <CircleAlert className="mt-0.5 h-4 w-4 shrink-0" />
            <span>{errorMessage}</span>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.div variants={fieldVariants} className="group space-y-2">
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
            disabled={isBusy}
            className="h-11 pl-10"
          />
        </div>
      </motion.div>

      <motion.div variants={fieldVariants} className="group space-y-2">
        <Label htmlFor="contrasena" className="text-slate-700">
          Contraseña
        </Label>
        <div className="relative">
          <Lock className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400 transition group-focus-within:text-corporativo-azul" />
          <Input
            id="contrasena"
            name="contrasena"
            type={mostrarContrasena ? "text" : "password"}
            autoComplete="current-password"
            placeholder="••••••••"
            value={contrasena}
            onChange={(event) => setContrasena(event.target.value)}
            disabled={isBusy}
            className="h-11 pr-11 pl-10"
          />
          <Button
            type="button"
            variant="ghost"
            size="icon-sm"
            onClick={() => setMostrarContrasena((visible) => !visible)}
            aria-label={mostrarContrasena ? "Ocultar contraseña" : "Mostrar contraseña"}
            className="absolute top-1/2 right-1.5 -translate-y-1/2 text-slate-400 hover:text-corporativo-azul"
          >
            {mostrarContrasena ? <EyeOff /> : <Eye />}
          </Button>
        </div>
      </motion.div>

      <motion.div variants={fieldVariants} className="-mt-2 text-right">
        <Link
          href="/recuperar-contrasena"
          className="text-sm font-medium text-corporativo-azul hover:underline"
        >
          ¿Olvidaste tu contraseña?
        </Link>
      </motion.div>

      <motion.div variants={fieldVariants}>
        <Button
          type="submit"
          disabled={isBusy}
          className="h-11 w-full bg-corporativo-azul text-base text-white hover:bg-corporativo-azul/90"
        >
          <AnimatePresence mode="wait" initial={false}>
            {isSuccess ? (
              <motion.span
                key="success"
                initial={shouldReduceMotion ? false : { scale: 0 }}
                animate={{ scale: 1 }}
                className="flex items-center gap-2"
              >
                <CircleCheck className="h-5 w-5" />
                Acceso concedido
              </motion.span>
            ) : isLoading ? (
              <motion.span key="loading" className="flex items-center gap-2">
                <LoaderCircle className="h-5 w-5 animate-spin" />
                Ingresando...
              </motion.span>
            ) : (
              <motion.span key="idle">Ingresar</motion.span>
            )}
          </AnimatePresence>
        </Button>
      </motion.div>
    </motion.form>
  );
}

/** Lee el mensaje `{ message }` de una respuesta de error; usa un texto genérico si no es válido. */
async function readErrorMessage(response: Response): Promise<string> {
  const body = (await response.json().catch(() => null)) as { message?: string } | null;
  return body?.message ?? DEFAULT_ERROR_MESSAGE;
}

/** Espera la cantidad de milisegundos indicada. */
function wait(milliseconds: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, milliseconds));
}
