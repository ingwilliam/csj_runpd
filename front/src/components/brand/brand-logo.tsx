import Image from "next/image";

/** Props del logo: `className` permite cambiar el tamaño según el lugar donde se use. */
type BrandLogoProps = {
  className?: string;
};

/** Logo institucional del RUNPD (`public/RUNPD.svg`). Por defecto usa el tamaño del login. */
export function BrandLogo({ className = "h-24 w-auto object-contain" }: BrandLogoProps) {
  return (
    <Image
      src="/RUNPD.svg"
      alt="Logo RUNPD"
      width={96}
      height={96}
      className={className}
      priority
    />
  );
}
