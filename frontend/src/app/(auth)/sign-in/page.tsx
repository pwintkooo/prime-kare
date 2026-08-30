import Image from "next/image";
import Link from "next/link";

export default function SignInPage() {
  return (
    <main className="min-h-screen bg-slate-950">
      <div className="grid min-h-screen lg:grid-cols-2">
        {/* Left - Image */}
        <div className="relative hidden lg:block">
          <Image
            src="/images/auth/login.jpg"
            alt="PrimeKare automotive workshop"
            fill
            priority
            className="object-cover"
          />

          {/* Overlay */}
          <div className="absolute inset-0 bg-slate-950/70" />

          {/* Content */}
          <div className="absolute inset-0 flex items-end p-12 xl:p-16">
            <div className="max-w-lg">
              <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
                PrimeKare
              </p>

              <h1 className="mt-4 text-4xl font-bold tracking-tight text-white xl:text-5xl">
                Professional care for
                <br />
                <span className="text-slate-400">every journey.</span>
              </h1>

              <p className="mt-6 max-w-md text-base leading-7 text-slate-200">
                Manage your vehicles, appointments, and service history with
                PrimeKare.
              </p>
            </div>
          </div>
        </div>

        {/* Right - Login */}
        <div className="flex items-center justify-center px-6 py-12 sm:px-8">
          <div className="w-full max-w-md">
            {/* Logo / Brand */}
            <div className="mb-10">
              <Link
                href="/"
                className="text-xl font-bold tracking-tight text-white"
              >
                Prime<span className="text-blue-400">Kare</span>
              </Link>
            </div>

            {/* Header */}
            <div>
              <h2 className="text-3xl font-bold tracking-tight text-white">
                Welcome back
              </h2>

              <p className="mt-2 text-sm text-slate-400">
                Sign in to your account to continue.
              </p>
            </div>

            {/* Form */}
            <form className="mt-8 space-y-5">
              {/* Email */}
              <div>
                <label
                  htmlFor="email"
                  className="mb-2 block text-sm font-medium text-slate-200"
                >
                  Email address
                </label>

                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  placeholder="you@example.com"
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                />
              </div>

              {/* Password */}
              <div>
                <div className="mb-2 flex items-center justify-between">
                  <label
                    htmlFor="password"
                    className="block text-sm font-medium text-slate-200"
                  >
                    Password
                  </label>

                  <Link
                    href="/forgot-password"
                    className="text-sm font-medium text-blue-400 transition hover:text-blue-300"
                  >
                    Forgot password?
                  </Link>
                </div>

                <input
                  id="password"
                  name="password"
                  type="password"
                  autoComplete="current-password"
                  placeholder="Enter your password"
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                />
              </div>

              {/* Remember me */}
              <div className="flex items-center gap-3">
                <input
                  id="remember"
                  name="remember"
                  type="checkbox"
                  className="h-4 w-4 rounded border-slate-700 bg-slate-900 text-blue-600 focus:ring-blue-400"
                />

                <label htmlFor="remember" className="text-sm text-slate-400">
                  Remember me
                </label>
              </div>

              {/* Submit */}
              <button
                type="submit"
                className="w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-offset-2 focus:ring-offset-slate-950"
              >
                Sign in
              </button>
            </form>

            {/* Register */}
            <p className="mt-8 text-center text-sm text-slate-400">
              Don&apos;t have an account?{" "}
              <Link
                href="/sign-up"
                className="font-semibold text-blue-400 transition hover:text-blue-300"
              >
                Create an account
              </Link>
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}