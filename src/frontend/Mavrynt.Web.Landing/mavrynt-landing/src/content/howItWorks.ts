import type { IconName } from "../components/Icon.tsx";

export interface HowStep {
  readonly id: "1" | "2" | "3";
  readonly icon: IconName;
}

export const howSteps: ReadonlyArray<HowStep> = [
  { id: "1", icon: "sparkles" },
  { id: "2", icon: "layers" },
  { id: "3", icon: "rocket" },
];
