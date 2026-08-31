interface PasswordRequirementsProps {
  password: string;
}

export default function PasswordRequirements({
  password,
}: PasswordRequirementsProps) {
  const requirements = {
    minLength: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    lowercase: /[a-z]/.test(password),
    number: /[0-9]/.test(password),
    special: /[^A-Za-z0-9]/.test(password),
  };

  return (
    <div className="mt-3 space-y-1.5 text-sm">
      <Requirement
        fulfilled={requirements.minLength}
        text="At least 8 characters"
      />

      <Requirement
        fulfilled={requirements.uppercase}
        text="One uppercase letter"
      />

      <Requirement
        fulfilled={requirements.lowercase}
        text="One lowercase letter"
      />

      <Requirement fulfilled={requirements.number} text="One number" />

      <Requirement
        fulfilled={requirements.special}
        text="One special character"
      />
    </div>
  );
}

interface RequirementProps {
  fulfilled: boolean;
  text: string;
}

function Requirement({ fulfilled, text }: RequirementProps) {
  return (
    <p className={fulfilled ? "text-green-400" : "text-slate-400"}>
      {fulfilled ? "✓" : "○"} {text}
    </p>
  );
}
