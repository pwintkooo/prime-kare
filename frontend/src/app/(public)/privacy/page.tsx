import { Card, CardContent } from "@/components/ui/card";

export default function PrivacyPolicyPage() {
  return (
    <main className="container mx-auto max-w-4xl px-4 py-10 sm:py-14">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
          Privacy Policy
        </h1>

        <p className="mt-2 text-sm text-muted-foreground">
          Last updated: September 2026
        </p>
      </div>

      <Card>
        <CardContent className="space-y-8 p-6 sm:p-8">
          <section>
            <p className="leading-7 text-muted-foreground">
              PrimeKare is a portfolio and demonstration project designed to
              showcase a car workshop booking and management system. This
              Privacy Policy provides a simple overview of how information may
              be handled when you use the application.
            </p>
          </section>

          <PolicySection title="Information We Collect">
            <p>
              When you use PrimeKare, you may provide information such as your
              name, email address, phone number, vehicle information, and
              booking details.
            </p>

            <p>
              This information is used to provide and demonstrate features
              within the application, such as managing your account, vehicles,
              and service bookings.
            </p>
          </PolicySection>

          <PolicySection title="How We Use Your Information">
            <p>Your information may be used to:</p>

            <ul className="list-disc space-y-2 pl-5">
              <li>Create and manage your account.</li>
              <li>Manage your registered vehicles.</li>
              <li>Create and manage workshop bookings.</li>
              <li>Display your booking and service history.</li>
              <li>Provide and improve application functionality.</li>
            </ul>
          </PolicySection>

          <PolicySection title="Data Storage">
            <p>
              Information entered into PrimeKare may be stored in the
              application&apos;s database or supporting cloud services used by
              the project.
            </p>

            <p>
              Please avoid entering sensitive, confidential, or real-world
              personal information when using this demonstration application.
            </p>
          </PolicySection>

          <PolicySection title="Third-Party Services">
            <p>
              PrimeKare may use third-party services for features such as
              authentication, cloud storage, hosting, or email notifications.
              These services may process limited information required to provide
              their functionality.
            </p>
          </PolicySection>

          <PolicySection title="Cookies and Authentication">
            <p>
              PrimeKare may use authentication tokens or similar technologies to
              keep users signed in and provide secure access to account
              features.
            </p>
          </PolicySection>

          <PolicySection title="Data Security">
            <p>
              Reasonable technical measures are used to protect information
              handled by the application. However, as PrimeKare is a portfolio
              and demonstration project, it should not be used to store
              sensitive or confidential information.
            </p>
          </PolicySection>

          <PolicySection title="Changes to This Policy">
            <p>
              This Privacy Policy may be updated as new features or services are
              added to PrimeKare.
            </p>
          </PolicySection>

          <PolicySection title="Contact">
            <p>
              If you have questions about this Privacy Policy or the PrimeKare
              project, please contact the project owner through the contact
              information provided with the application.
            </p>
          </PolicySection>
        </CardContent>
      </Card>
    </main>
  );
}

function PolicySection({
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