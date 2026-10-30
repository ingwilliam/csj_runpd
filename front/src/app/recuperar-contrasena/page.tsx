import Link from "next/link";
import { BrandLogo } from "@/components/brand/brand-logo";
import { RecuperarContrasenaForm } from "./recuperar-contrasena-form";

/** Página para solicitar una contraseña temporal por correo. */
export default function RecuperarContrasenaPage() {
  return (
    <main className="relative flex min-h-screen items-center justify-center overflow-hidden bg-corporativo-azul px-4 py-12">
      <div
        aria-hidden="true"
        className="absolute inset-0 bg-[radial-gradient(circle,rgba(255,255,255,0.10)_1px,transparent_1px)] bg-[length:24px_24px]"
      />

      <section className="relative w-full max-w-md animate-entrada overflow-hidden rounded-2xl bg-white shadow-2xl">
        <div className="flex h-2">
          <div className="flex-1 bg-corporativo-verde" />
          <div className="flex-1 bg-corporativo-amarillo" />
          <div className="flex-1 bg-corporativo-azul" />
        </div>

        <div className="space-y-8 p-8">
          <header className="flex flex-col items-center gap-5 text-center">
            <BrandLogo />
            <h1 className="max-w-xs text-lg font-semibold leading-snug text-corporativo-azul">
              Registro Único Nacional de Procesos de Descongestión
            </h1>
          </header>

          <RecuperarContrasenaForm />

          <p className="text-center text-sm text-slate-600">
            <Link href="/login" className="font-medium text-corporativo-azul hover:underline">
              Volver al inicio de sesión
            </Link>
          </p>
        </div>
      </section>
    </main>
  );
}
