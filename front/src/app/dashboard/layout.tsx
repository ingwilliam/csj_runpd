import { PageContainer } from "@/components/page-container";
import { DashboardHeader } from "./dashboard-header";
import { RouteFocus } from "./route-focus";

/** Layout de las pantallas privadas: cabecera compartida y contenido accesible. */
export default function DashboardLayout({ children }: LayoutProps<"/dashboard">) {
  return (
    <div className="flex min-h-screen flex-col bg-slate-100">
      {/* Enlace de salto: solo se muestra al recibir foco con el teclado. */}
      <a
        href="#contenido"
        className="sr-only focus:not-sr-only focus:absolute focus:top-3 focus:left-3 focus:z-50 focus:rounded-lg focus:bg-white focus:px-4 focus:py-2 focus:text-sm focus:font-medium focus:text-corporativo-azul focus:shadow-lg"
      >
        Saltar al contenido principal
      </a>

      <DashboardHeader />
      <RouteFocus />

      <main id="contenido" tabIndex={-1} className="flex-1 outline-none">
        <PageContainer>{children}</PageContainer>
      </main>
    </div>
  );
}
