import { Suspense } from "react";
import { DashboardHeader } from "./dashboard-header";
import { DashboardModules } from "./module-cards";
import { WelcomeBanner } from "./welcome-banner";

/** Panel principal: muestra los módulos permitidos según los roles del usuario. */
export default function DashboardPage() {
  return (
    <main className="min-h-screen bg-slate-100">
      <DashboardHeader />
      <section className="space-y-6 p-6">
        <WelcomeBanner />
        <Suspense fallback={null}>
          <DashboardModules />
        </Suspense>
      </section>
    </main>
  );
}
