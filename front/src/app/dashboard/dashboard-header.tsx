import Link from "next/link";
import { UserRound, LayoutDashboard } from "lucide-react";
import { BrandLogo } from "@/components/brand/brand-logo";
import { Button } from "@/components/ui/button";
import { LogoutButton } from "./logout-button";

/** Cabecera de las pantallas privadas: logo, navegación y cierre de sesión. */
export function DashboardHeader() {
  return (
    <header className="flex items-center justify-between gap-4 bg-corporativo-azul px-6 py-3 text-white">
      {/* El logo tiene trazos azules: va sobre una tarjeta blanca para que se vea en la franja azul. */}
      <div className="rounded-lg bg-white px-3 py-2">
        <BrandLogo className="h-12 w-auto object-contain" />
      </div>
      <nav className="flex items-center gap-2">
        <Button asChild variant="ghost" className="text-white hover:bg-white/10 hover:text-white">
          <Link href="/dashboard">
            <LayoutDashboard />
            Inicio
          </Link>
        </Button>
        <Button asChild variant="ghost" className="text-white hover:bg-white/10 hover:text-white">
          <Link href="/dashboard/perfil">
            <UserRound />
            Mi perfil
          </Link>
        </Button>
        <LogoutButton />
      </nav>
    </header>
  );
}
