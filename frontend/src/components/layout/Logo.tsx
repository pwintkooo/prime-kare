import Link from "next/link";

export default function Logo() {
  return (
    <Link href="/" className="flex items-center gap-2">
      <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-600 text-white font-bold">
        PK
      </div>

      <div className="hidden sm:block">
        <h1 className="text-lg font-bold text-slate-900">Prime Kare</h1>

        <p className="text-xs text-slate-500">Workshop Management</p>
      </div>
    </Link>
  );
}
