import { Suspense } from "react";
import type { Metadata } from "next";
import { PageBreadcrumb } from "../page-breadcrumb";
import { ProfileDetails } from "./profile-details";
import { ProfileSkeleton } from "./profile-skeleton";

export const metadata: Metadata = {
  title: "Mi perfil | RUNPD",
};

/** Página "Mi perfil": datos de la sesión actual. */
export default function ProfilePage() {
  return (
    <div>
      <PageBreadcrumb items={[{ label: "Mi perfil" }]} />

      <Suspense fallback={<ProfileSkeleton />}>
        <ProfileDetails />
      </Suspense>
    </div>
  );
}
