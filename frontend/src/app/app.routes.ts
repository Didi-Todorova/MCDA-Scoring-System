import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home')
        .then(m => m.HomeComponent)
  },

  {
    path: 'decisions',
    loadComponent: () =>
      import('./features/decisions/decision-list/decision-list')
        .then(m => m.DecisionListComponent)
  },

  {
  path: 'decisions/new',
    loadComponent: () =>
      import('./features/decisions/decision-create/decision-create')
        .then(m => m.DecisionCreateComponent)
  },

  {
    path: 'decisions/:id/wizard',
    loadComponent: () =>
      import('./features/decisions/decision-wizard/decision-wizard')
        .then(m => m.DecisionWizardComponent),

    children: [
      {
        path: '',
        redirectTo: 'basic',
        pathMatch: 'full'
      },

      {
        path: 'basic',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/basic-information/basic-information')
            .then(m => m.BasicInformationComponent)
      },

      {
        path: 'criteria',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/criteria/criteria')
            .then(m => m.CriteriaComponent)
      },

      {
        path: 'weighting',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/weighting/weighting')
            .then(m => m.WeightingComponent)
      },

      {
        path: 'alternatives',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/alternatives/alternatives')
            .then(m => m.AlternativesComponent)
      },

      {
        path: 'preview',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/preview/preview')
            .then(m => m.PreviewComponent)
      },

      {
        path: 'results',
        loadComponent: () =>
          import('./features/decisions/decision-wizard/steps/results/results')
            .then(m => m.ResultsComponent)
      }
    ]
  }
];