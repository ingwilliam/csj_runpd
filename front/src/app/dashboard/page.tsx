import { Suspense } from "react";
import type { Metadata } from "next";
import { DashboardModules } from "./module-cards";
import { ModuleCardsSkeleton } from "./module-cards-skeleton";
import { WelcomeBanner } from "./welcome-banner";

export const metadata: Metadata = {
  title: "Panel | RUNPD",
};

/** Panel principal: muestra los módulos permitidos según los roles del usuario. */
export default function DashboardPage() {
  return (
    <div className="space-y-6">
      <WelcomeBanner />
      <Suspense fallback={<ModuleCardsSkeleton />}>
        <DashboardModules />
      </Suspense>
    </div>
  );
}
