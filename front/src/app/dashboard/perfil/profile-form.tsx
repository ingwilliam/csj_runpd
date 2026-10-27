"use client";

import { useActionState, useEffect } from "react";
import { motion, useReducedMotion } from "motion/react";
import { Lock, Save, LoaderCircle } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import type { Perfil } from "@/lib/backend/types";
import { actualizarPerfilAction, type PerfilActionState } from "./actions";

const ESTADO_INICIAL: PerfilActionState = { ok: false, message: null };

type ProfileFormProps = {
  perfil: Perfil;
};

/** Formulario de datos personales. El correo se muestra pero no se puede cambiar. */
export function ProfileForm({ perfil }: ProfileFormProps) {
  const shouldReduceMotion = useReducedMotion();
  const [estado, accion, isPending] = useActionState(actualizarPerfilAction, ESTADO_INICIAL);

  // Notifica cada resultado de guardado con un toast.
  useEffect(() => {
    if (!estado.message) {
      return;
    }
    if (estado.ok) {
      toast.success(estado.message);
    } else {
      toast.error(estado.message);
    }
  }, [estado]);

  return (
    <motion.section
      initial={shouldReduceMotion ? false : { opacity: 0, y: 16 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.45, ease: "easeOut" }}
      className="rounded-2xl border border-corporativo-azul/10 bg-white p-6 shadow-sm"
    >
      <header className="mb-6 space-y-1">
        <h2 className="text-lg font-semibold text-corporativo-azul">Datos personales</h2>
        <p className="text-sm text-slate-500">Actualiza tu nombre y tus apellidos.</p>
      </header>

      <form action={accion} className="space-y-5">
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="space-y-2">
            <Label htmlFor="primerNombre">Primer nombre</Label>
            <Input
              id="primerNombre"
              name="primerNombre"
              defaultValue={perfil.primerNombre}
              maxLength={100}
              required
              disabled={isPending}
              className="h-11"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="segundoNombre">Segundo nombre</Label>
            <Input
              id="segundoNombre"
              name="segundoNombre"
              defaultValue={perfil.segundoNombre ?? ""}
              maxLength={100}
              disabled={isPending}
              className="h-11"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="primerApellido">Primer apellido</Label>
            <Input
              id="primerApellido"
              name="primerApellido"
              defaultValue={perfil.primerApellido}
              maxLength={100}
              required
              disabled={isPending}
              className="h-11"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="segundoApellido">Segundo apellido</Label>
            <Input
              id="segundoApellido"
              name="segundoApellido"
              defaultValue={perfil.segundoApellido ?? ""}
              maxLength={100}
              disabled={isPending}
              className="h-11"
            />
          </div>
        </div>

        <div className="space-y-2">
          <Label htmlFor="correoElectronico">Correo electrónico</Label>
          <div className="relative">
            <Lock className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <Input
              id="correoElectronico"
              value={perfil.correoElectronico}
              disabled
              readOnly
              className="h-11 bg-slate-50 pl-10"
            />
          </div>
          <p className="text-xs text-slate-500">
            El correo es tu usuario de inicio de sesión y no se puede cambiar desde aquí.
          </p>
        </div>

        <div className="flex justify-end">
          <Button
            type="submit"
            disabled={isPending}
            className="h-11 bg-corporativo-azul px-6 text-white hover:bg-corporativo-azul/90"
          >
            {isPending ? <LoaderCircle className="animate-spin" /> : <Save />}
            {isPending ? "Guardando..." : "Guardar cambios"}
          </Button>
        </div>
      </form>
    </motion.section>
  );
}
