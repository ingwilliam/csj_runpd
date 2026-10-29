"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, UserRound } from "lucide-react";
import { BrandLogo } from "@/components/brand/brand-logo";
import { PageContainer } from "@/components/page-container";
import { Button } from "@/components/ui/button";
import { LogoutButton } from "./logout-button";

/** Cabecera fija de las pantallas privadas: logo, navegación y cierre de sesión. */
export function DashboardHeader() {
  const pathname = usePathname();
  const enPanel = pathname === "/dashboard";
  const enPerfil = pathname === "/dashboard/perfil";

  return (
    <header className="sticky top-0 z-40 bg-corporativo-azul text-white shadow-md">
      <PageContainer className="flex items-center justify-between gap-3 py-3">
        {/* El logo tiene trazos azules: va sobre una tarjeta blanca para que se vea en la franja azul. */}
        <Link
          href="/dashboard"
          aria-label="Ir al panel principal"
          className="rounded-lg bg-white px-2 py-1.5 transition-opacity hover:opacity-90 sm:px-3 sm:py-2"
        >
          <BrandLogo className="h-10 w-auto object-contain sm:h-12" />
        </Link>

        <nav aria-label="Navegación principal" className="flex items-center gap-1 sm:gap-2">
          <Button asChild variant="ghost" className={navLinkClass(enPanel)}>
            <Link
              href="/dashboard"
              aria-label="Ir al panel"
              aria-current={enPanel ? "page" : undefined}
              title="Panel"
            >
              <LayoutDashboard />
              <span className="hidden sm:inline">Panel</span>
            </Link>
          </Button>

          <Button asChild variant="ghost" className={navLinkClass(enPerfil)}>
            <Link
              href="/dashboard/perfil"
              aria-label="Ir a mi perfil"
              aria-current={enPerfil ? "page" : undefined}
              title="Mi perfil"
            >
              <UserRound />
              <span className="hidden sm:inline">Mi perfil</span>
            </Link>
          </Button>

          <LogoutButton />
        </nav>
      </PageContainer>
    </header>
  );
}

/** Clases del enlace de navegación según si corresponde a la ruta actual. */
function navLinkClass(activo: boolean): string {
  return activo
    ? "bg-white/15 text-white hover:bg-white/20 hover:text-white"
    : "text-white/85 hover:bg-white/10 hover:text-white";
}
