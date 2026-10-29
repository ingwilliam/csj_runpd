import type { Metadata } from "next";
import Link from "next/link";
import { BrandLogo } from "@/components/brand/brand-logo";
import { Button } from "@/components/ui/button";

export const metadata: Metadata = {
  title: "Página no encontrada | RUNPD",
};

/** Página 404 para rutas que no existen. */
export default function NotFound() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-100 px-4 py-12">
      <section className="w-full max-w-md overflow-hidden rounded-2xl border border-corporativo-azul/10 bg-white p-8 text-center shadow-xl">
        <div className="mx-auto w-fit rounded-xl bg-slate-50 p-3">
          <BrandLogo className="h-16 w-auto object-contain" />
        </div>

        <p className="mt-6 text-5xl font-bold text-corporativo-azul">404</p>
        <h1 className="mt-3 text-xl font-semibold text-slate-800">Página no encontrada</h1>
        <p className="mt-2 text-sm text-slate-600">
          La página que buscas no existe o cambió de lugar.
        </p>

        <Button
          asChild
          className="mt-6 h-11 w-full bg-corporativo-azul text-white hover:bg-corporativo-azul/90"
        >
          <Link href="/dashboard">Ir al panel</Link>
        </Button>
      </section>
    </main>
  );
}
