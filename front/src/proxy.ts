import { NextResponse, type NextRequest } from "next/server";
import { SESSION_COOKIE_NAME } from "@/lib/auth/session";

/**
 * Protege las rutas privadas: sin cookie de sesión redirige a /login.
 * Solo comprueba la presencia de la cookie; la validez del token la decide el backend.
 */
export function proxy(request: NextRequest) {
  // La raíz envía al panel; el propio panel decide si hay que ir a /login.
  if (request.nextUrl.pathname === "/") {
    return NextResponse.redirect(new URL("/dashboard", request.url));
  }

  if (request.cookies.has(SESSION_COOKIE_NAME)) {
    return NextResponse.next();
  }

  const loginUrl = new URL("/login", request.url);
  return NextResponse.redirect(loginUrl);
}

export const config = {
  matcher: ["/", "/dashboard", "/dashboard/:path*"],
};
