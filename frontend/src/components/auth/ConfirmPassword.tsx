interface ConfirmPasswordProps {
  password: string;
  confirmPassword: string;
}

export default function ConfirmPassword({
  password,
  confirmPassword,
}: ConfirmPasswordProps) {
  if (!confirmPassword) return null;

  return (
    <p
      className={`mt-2 text-sm ${
        password === confirmPassword ? "text-green-400" : "text-red-400"
      }`}
    >
      {password === confirmPassword
        ? "✓ Passwords match"
        : "✕ Passwords do not match"}
    </p>
  );
}
