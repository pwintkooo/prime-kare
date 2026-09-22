import { Card, CardContent } from "@/components/ui/card";

export default function TermsPage() {
  return (
    <main className="container mx-auto max-w-4xl px-4 py-10 sm:py-14">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
          Terms & Conditions
        </h1>

        <p className="mt-2 text-sm text-muted-foreground">
          Last updated: September 2026
        </p>
      </div>

      <Card>
        <CardContent className="space-y-8 p-6 sm:p-8">
          <section>
            <p className="leading-7 text-muted-foreground">
              Welcome to PrimeKare. By using this application, you agree to the
              following basic terms.
            </p>
          </section>

          <TermsSection title="About PrimeKare">
            <p>
              PrimeKare is a portfolio and demonstration project created to
              showcase a car workshop booking and management system.
            </p>

            <p>
              Unless otherwise stated, bookings, services, prices, vehicles,
              workshop information, and other content displayed within the
              application are for demonstration purposes only.
            </p>
          </TermsSection>

          <TermsSection title="Using the Application">
            <p>
              You may use PrimeKare to explore features such as creating an
              account, registering vehicles, viewing workshop services, and
              creating or managing bookings.
            </p>

            <p>
              Please use the application responsibly and do not attempt to
              misuse, disrupt, damage, or gain unauthorized access to the
              application or its services.
            </p>
          </TermsSection>

          <TermsSection title="Accounts">
            <p>
              You are responsible for the information entered through your
              account. Since PrimeKare is a demonstration application, you
              should avoid providing sensitive or confidential information.
            </p>
          </TermsSection>

          <TermsSection title="Bookings">
            <p>
              Bookings created through the public demonstration version of
              PrimeKare are for demonstration purposes and should not be
              considered real workshop appointments unless explicitly stated
              otherwise.
            </p>
          </TermsSection>

          <TermsSection title="Service Information">
            <p>
              Service descriptions, prices, appointment availability, estimated
              service durations, and other workshop-related information may be
              fictional or provided for demonstration purposes.
            </p>

            <p>
              They should not be treated as actual quotations, professional
              automotive advice, or confirmed services.
            </p>
          </TermsSection>

          <TermsSection title="Availability">
            <p>
              PrimeKare may be changed, temporarily unavailable, reset, or
              discontinued at any time as the project is developed and
              maintained.
            </p>
          </TermsSection>

          <TermsSection title="Limitation of Liability">
            <p>
              PrimeKare is provided as a demonstration project without
              guarantees regarding availability, accuracy, or suitability for
              any particular purpose.
            </p>

            <p>
              The application should not be relied upon for actual vehicle
              servicing, payments, or other important decisions.
            </p>
          </TermsSection>

          <TermsSection title="Changes to These Terms">
            <p>
              These Terms & Conditions may be updated as PrimeKare develops and
              new features are introduced.
            </p>

            <p>
              By continuing to use the application after changes are made, you
              acknowledge the updated terms.
            </p>
          </TermsSection>
        </CardContent>
      </Card>
    </main>
  );
}

function TermsSection({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <section className="space-y-3">
      <h2 className="text-xl font-semibold">{title}</h2>

      <div className="space-y-3 leading-7 text-muted-foreground">
        {children}
      </div>
    </section>
  );
}