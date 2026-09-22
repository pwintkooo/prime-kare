import Link from "next/link";
import Image from "next/image";

type LogoProps = {
  className?: string;
};

export default function Logo({ className = "h-10" }: LogoProps) {
  return (
    <div>
      <Link
        href="/"
        className="inline-flex items-center"
        aria-label="PrimeKare home"
      >
        <Image
          src="/images/branding/primekare-white-logo.png"
          alt="PrimeKare Logo"
          width={180}
          height={54}
          className={`${className} w-auto object-contain`}
          priority
        />
      </Link>
      <p className="text-xs text-slate-500">Workshop Management</p>
    </div>
  );
}
