import { PoolLength } from './personal-best';

export interface Meet {
  id: number;
  name: string;
  city: string | null;
  date: string;
  poolLength: PoolLength;
  resultCount: number;
}
