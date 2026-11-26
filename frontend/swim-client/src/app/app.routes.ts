import { Routes } from '@angular/router';
import { LeaderboardComponent } from './features/leaderboard/leaderboard.component';
import { RelayComponent } from './features/relay/relay.component';
import { ResultDetailComponent } from './features/results/result-detail.component';
import { ResultFormComponent } from './features/results/result-form.component';
import { SwimmerDetailComponent } from './features/swimmers/swimmer-detail.component';
import { SwimmersListComponent } from './features/swimmers/swimmers-list.component';

export const routes: Routes = [
  { path: 'swimmers', component: SwimmersListComponent },
  { path: 'swimmers/:id', component: SwimmerDetailComponent },
  { path: 'leaderboard', component: LeaderboardComponent },
  { path: 'relay', component: RelayComponent },
  // 'results/new' trebuie înaintea lui 'results/:id'
  { path: 'results/new', component: ResultFormComponent },
  { path: 'results/:id', component: ResultDetailComponent },
  { path: '', redirectTo: 'swimmers', pathMatch: 'full' },
];