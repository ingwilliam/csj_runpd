import type { Metadata } from "next";
import { Toaster } from "sonner";
import "./globals.css";

export const metadata: Metadata = {
  title: "RUNPD | Rama Judicial",
  description:
    "Registro Único Nacional de Procesos de Descongestión de la Rama Judicial",
};

/** Layout raíz: define el idioma del documento y el contenedor de las páginas. */
export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="es" className="h-full">
      <body className="flex min-h-full flex-col">
        {children}
        <Toaster position="top-center" richColors />
      </body>
    </html>
  );
}
