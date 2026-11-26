import { PoolLength, Stroke } from '../models/personal-best';
import { PacingProfile } from '../models/result';

// 55230 -> "55.23", 125400 -> "2:05.40"
export function formatTime(ms: number): string {
  const totalCs = Math.round(ms / 10);
  const cs = (totalCs % 100).toString().padStart(2, '0');
  const totalSec = Math.floor(totalCs / 100);
  const sec = totalSec % 60;
  const min = Math.floor(totalSec / 60);
  return min > 0 ? `${min}:${sec.toString().padStart(2, '0')}.${cs}` : `${sec}.${cs}`;
}

// "55.23" -> 55230, "1:55.4" -> 115400, "2:05" -> 125000; null dacă formatul e greșit
export function parseTime(text: string): number | null {
  const match = /^(?:(\d{1,2}):)?(\d{1,2})(?:[.,](\d{1,2}))?$/.exec(text.trim());
  if (!match) return null;

  const minutes = match[1] ? Number(match[1]) : 0;
  const seconds = Number(match[2]);
  const centis = match[3] ? Number(match[3].padEnd(2, '0')) : 0;

  if (seconds >= 60) return null;
  const ms = (minutes * 60 + seconds) * 1000 + centis * 10;
  return ms > 0 ? ms : null;
}

const strokeNames: Record<Stroke, string> = {
  Freestyle: 'Liber',
  Backstroke: 'Spate',
  Breaststroke: 'Bras',
  Butterfly: 'Fluture',
  Medley: 'Mixt',
};

export function strokeLabel(stroke: Stroke): string {
  return strokeNames[stroke];
}

export function eventLabel(distanceMeters: number, stroke: Stroke): string {
  return `${distanceMeters}m ${strokeNames[stroke]}`;
}

export function poolLabel(pool: PoolLength): string {
  return pool === 'Long' ? '50m' : '25m';
}

export function profileLabel(profile: PacingProfile): string {
  switch (profile) {
    case 'PositiveSplit':
      return 'A plecat prea tare (a doua jumătate mai lentă)';
    case 'NegativeSplit':
      return 'Negative split (a doua jumătate mai rapidă)';
    default:
      return 'Ritm constant';
  }
}
