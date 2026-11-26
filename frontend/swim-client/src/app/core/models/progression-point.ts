import { PoolLength } from './personal-best';

export interface ProgressionPoint {
  date: string;
  meetName: string;
  poolLength: PoolLength;
  timeMs: number;
}
