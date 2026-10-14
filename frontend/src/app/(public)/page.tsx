import Hero from "@/components/home/Hero";
import Services from "@/components/home/Services";
import WhyChooseUs from "@/components/home/WhyChooseUs";
import PrimeKareChat from "@/components/ai/PrimeKareChat";

export default function HomePage() {
  return (
    <>
      <PrimeKareChat />
      <Hero />
      <Services />
      <WhyChooseUs />
    </>
  );
}
