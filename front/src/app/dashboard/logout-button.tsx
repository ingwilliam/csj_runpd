"use client";

import { useState, useTransition } from "react";
import { LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { logout } from "./actions";

/**
 * Botón de cierre de sesión con confirmación.
 * El diálogo se cierra antes de llamar a la server action: si la navegación ocurre mientras
 * el diálogo sigue montado, Radix deja bloqueados los clics de la página (pointer-events: none).
 */
export function LogoutButton() {
  const [isOpen, setIsOpen] = useState(false);
  const [isPending, startTransition] = useTransition();

  /** Cierra el diálogo y, después, ejecuta el cierre de sesión en el servidor. */
  function confirmLogout() {
    setIsOpen(false);
    startTransition(() => logout());
  }

  return (
    <AlertDialog open={isOpen} onOpenChange={setIsOpen}>
      <AlertDialogTrigger asChild>
        <Button
          variant="outline"
          disabled={isPending}
          aria-label="Cerrar sesión"
          title="Cerrar sesión"
          className="border-white/30 bg-transparent text-white hover:bg-white/10 hover:text-white"
        >
          <LogOut />
          <span className="hidden sm:inline">Cerrar sesión</span>
        </Button>
      </AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>¿Cerrar sesión?</AlertDialogTitle>
          <AlertDialogDescription>
            Tendrás que volver a iniciar sesión para acceder al panel.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancelar</AlertDialogCancel>
          <AlertDialogAction onClick={confirmLogout}>Cerrar sesión</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
