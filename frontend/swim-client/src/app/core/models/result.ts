import { PoolLength, Stroke } from './personal-best';
import { Gender } from './swimmer';

export interface Split {
  distanceMeters: number;
  cumulativeTimeMs: number;
}

export interface SwimResult {
  id: number;
  swimmerId: number;
  swimmerName: string;
  eventId: number;
  stroke: Stroke;
  distanceMeters: number;
  gender: Gender;
  meetId: number;
  meetName: string;
  meetDate: string;
  poolLength: PoolLength;
  timeMs: number;
  splits: Split[];
}

export interface Points {
  resultId: number;
  timeMs: number;
  baseTimeMs: number;
  points: number;
}

export type PacingProfile = 'Even' | 'PositiveSplit' | 'NegativeSplit';

export interface Segment {
  fromMeters: number;
  toMeters: number;
  timeMs: number;
  per50Ms: number;
}

export interface Pacing {
  resultId: number;
  firstHalfMs: number;
  secondHalfMs: number;
  differencePercent: number;
  profile: PacingProfile;
  segments: Segment[];
}

export interface CreateResultRequest {
  swimmerId: number;
  eventId: number;
  meetId: number;
  timeMs: number;
  splits: Split[];
}
