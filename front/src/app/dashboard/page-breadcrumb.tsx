import Link from "next/link";
import { ChevronRight, LayoutDashboard } from "lucide-react";

type BreadcrumbItem = {
  label: string;
  href?: string;
};

type PageBreadcrumbProps = {
  items: BreadcrumbItem[];
};

/** Migas de pan de las pantallas internas: permiten volver al panel y ubicar la sección actual. */
export function PageBreadcrumb({ items }: PageBreadcrumbProps) {
  return (
    <nav aria-label="Migas de pan" className="mb-5 flex flex-wrap items-center gap-1.5 text-sm">
      <Link
        href="/dashboard"
        className="inline-flex items-center gap-1.5 rounded-md px-1.5 py-1 font-medium text-corporativo-azul transition-colors hover:bg-corporativo-azul/5 hover:underline"
      >
        <LayoutDashboard aria-hidden="true" className="h-4 w-4" />
        Panel
      </Link>

      {items.map((item, index) => (
        <span key={`${item.label}-${index}`} className="flex items-center gap-1.5">
          <ChevronRight aria-hidden="true" className="h-3.5 w-3.5 text-slate-400" />
          {item.href ? (
            <Link
              href={item.href}
              className="rounded-md px-1.5 py-1 font-medium text-corporativo-azul transition-colors hover:bg-corporativo-azul/5 hover:underline"
            >
              {item.label}
            </Link>
          ) : (
            <span aria-current="page" className="px-1.5 py-1 text-slate-600">
              {item.label}
            </span>
          )}
        </span>
      ))}
    </nav>
  );
}
