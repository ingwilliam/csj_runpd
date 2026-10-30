import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  // Permite que el servidor de desarrollo acepte peticiones desde los orígenes usados para compartir la demo.
  allowedDevOrigins: ["*.ngrok-free.app", "192.168.100.86"],
  cacheComponents: true,
  partialPrefetching: true,
  turbopack: {
    rules: {
      "*.css": {
        loaders: ["@tailwindcss/turbopack"],
        as: "*.css",
      },
    },
  },
};

export default nextConfig;
