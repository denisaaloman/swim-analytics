import { Stroke } from './personal-best';
import { Gender } from './swimmer';

export interface SwimEvent {
  id: number;
  stroke: Stroke;
  distanceMeters: number;
  gender: Gender;
}
