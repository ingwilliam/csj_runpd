import { Suspense } from "react";
import { DashboardHeader } from "../dashboard-header";
import { ProfileDetails } from "./profile-details";

/** Página "Mi perfil": datos de la sesión actual. */
export default function ProfilePage() {
  return (
    <main className="min-h-screen bg-slate-100">
      <DashboardHeader />
      <section className="p-6">
        <Suspense fallback={null}>
          <ProfileDetails />
        </Suspense>
      </section>
    </main>
  );
}
