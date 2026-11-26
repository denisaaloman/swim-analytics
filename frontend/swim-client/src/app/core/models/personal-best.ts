import { Gender } from './swimmer';

export type Stroke = 'Freestyle' | 'Backstroke' | 'Breaststroke' | 'Butterfly' | 'Medley';
export type PoolLength = 'Short' | 'Long';

export interface PersonalBest {
  eventId: number;
  stroke: Stroke;
  distanceMeters: number;
  gender: Gender;
  poolLength: PoolLength;
  bestTimeMs: number;
}
