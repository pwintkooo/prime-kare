import type { ReactNode } from "react";

interface Props {
  children: ReactNode;
}

export default function PublicLayout({ children }: Props) {
  return (
    <>
      <main>{children}</main>
    </>
  );
}