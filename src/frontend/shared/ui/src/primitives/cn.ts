/**
 * Tiny `cn` helper — wraps `clsx` so primitives can be terse.
 * Centralized here so we can swap implementations later (e.g. add
 * tailwind-merge) without touching every component.
 */
import clsx, { type ClassValue } from "clsx";

export const cn = (...classes: ClassValue[]): string => clsx(classes);
