export interface LeaderboardEntry {
  rank: number;
  swimmerId: number;
  swimmerName: string;
  clubName: string | null;
  bestTimeMs: number;
}
