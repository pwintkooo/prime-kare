import type { ReactNode } from "react";

import Navbar from "@/components/layout/Navbar";
import Footer from "@/components/layout/Footer";
import PrimeKareChat from "@/components/ai/PrimeKareChat";

interface Props {
  children: ReactNode;
}

export default function PublicLayout({ children }: Props) {
  return (
    <>
      <Navbar />
      <main>{children}</main>
      <PrimeKareChat />
      <Footer />
    </>
  );
}
