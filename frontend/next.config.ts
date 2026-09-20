import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  images: {
    remotePatterns: [
      //Azure Blob Storage
      {
        protocol: "https",
        hostname: "primekarestorage.blob.core.windows.net",
      },
      //Firebase / Google Cloud Storage
      {
        protocol: "https",
        hostname: "storage.googleapis.com",
      },
    ],
  },
};

export default nextConfig;