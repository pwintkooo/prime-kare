"use client";

import Image, { ImageProps } from "next/image";
import { useState } from "react";

interface ImageWithFallbackProps extends Omit<ImageProps, "src"> {
  src?: string | null;
  fallbackSrc: string;
}

function FallbackImage({
  src,
  fallbackSrc,
  alt,
  ...props
}: ImageWithFallbackProps) {
  const [hasError, setHasError] = useState(false);

  return (
    <Image
      {...props}
      src={!src || hasError ? fallbackSrc : src}
      alt={alt}
      onError={() => setHasError(true)}
    />
  );
}

export function ImageWithFallback(props: ImageWithFallbackProps) {
  return <FallbackImage key={props.src || props.fallbackSrc} {...props} />;
}