import { Stroke } from './personal-best';

export interface RelayLeg {
  stroke: Stroke;
  swimmerId: number;
  swimmerName: string;
  timeMs: number;
}

export interface Relay {
  totalTimeMs: number;
  legs: RelayLeg[];
}
