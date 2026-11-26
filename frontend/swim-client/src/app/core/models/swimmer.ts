export type Gender = 'Male' | 'Female';

export interface Swimmer {
  id: number;
  firstName: string;
  lastName: string;
  birthDate: string; // "2005-03-12"
  gender: Gender;
  clubName: string | null;
}
