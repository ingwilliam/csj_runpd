"use server";

import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { SESSION_COOKIE_NAME } from "@/lib/auth/session";

/** Cierra la sesión borrando la cookie de sesión y devuelve al usuario al login. */
export async function logout() {
  const cookieStore = await cookies();
  cookieStore.delete({ name: SESSION_COOKIE_NAME, path: "/" });
  redirect("/login");
}
